using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Caliburn.Micro;
using Newtonsoft.Json;
using ShTaskerAndBot.Manager;
using ShTaskerAndBot.Models;
using ShTaskerAndBot.Utils;

namespace ShTaskerAndBot.ViewModels
{
    public class ShellViewModel : Screen
    {
        private readonly IWindowManager windowManager;
        private readonly KeyboardHook keyboardHook;
        private const string WorkingLabel = "Working";
        private const string NotWorkingLabel = "Not working";
        private const string NameOfConfigFile = "config.json";
        private const string ResourceAddMenu = "CmAdd";
        private static readonly SolidColorBrush workingBrush = new SolidColorBrush(Colors.DarkGreen);
        private static readonly SolidColorBrush notWorkingBrush = new SolidColorBrush(Colors.DarkMagenta);
        private readonly string[] processing = {"|", "/", "-", "\\"};
        private Entry selectedItem;
        private byte counter;
        private string status = NotWorkingLabel;
        private string processingLabel = ".";
        private SolidColorBrush statusColor = notWorkingBrush;

        public BotManager BotManager { get; private set; }
        public Configuration Configuration { get; private set; }

        public bool LimitToActiveWindow { get; set; } = true;
        public string Title { get; } = "ShTasker&Bot";
        public int SelectedIndex { get; set; }


        public bool UseBuiltinSendWait
        {
            get => Configuration.UseBuiltinSendWait;
            set { Configuration.UseBuiltinSendWait = value; }
        }


        public bool GlobalShortcutEnabled
        {
            get => Configuration.GlobalShortcutEnabled;
            set
            {
                Configuration.GlobalShortcutEnabled = value;
                ChangeGlobalShortcutState(value);
                NotifyOfPropertyChange(() => GlobalShortcutEnabled);
            }
        }

        public string Processing
        {
            get => processingLabel;
            set
            {
                processingLabel = value;
                NotifyOfPropertyChange(() => Processing);
            }
        }

        public string Status
        {
            get => status;
            set
            {
                status = value;
                NotifyOfPropertyChange(() => Status);
            }
        }

        public SolidColorBrush StatusColor
        {
            get => statusColor;
            set
            {
                statusColor = value;
                NotifyOfPropertyChange(() => StatusColor);
            }
        }

        public string ProcessName
        {
            get => Configuration?.ProcessName;
            set
            {
                Configuration.ProcessName = value;
                NotifyOfPropertyChange(() => ProcessName);
            }
        }

        public string IntervalValue
        {
            get => Configuration?.Interval.ToString();
            set
            {
                Configuration.Interval = string.IsNullOrWhiteSpace(value) || value == "" ? 0 : Convert.ToInt32(value);
                NotifyOfPropertyChange(() => IntervalValue);
            }
        }


        public string Shortcut
        {
            get => Configuration.Shortcut?.ToString() ?? "";
        }


        public BindableCollection<Entry> Items
        {
            get => Configuration?.Items;
            set
            {
                if (Configuration != null) Configuration.Items = value;
                NotifyOfPropertyChange(() => Items);
            }
        }

        public Entry SelectedItem
        {
            get => selectedItem;
            set
            {
                selectedItem = value;
                NotifyOfPropertyChange(() => SelectedItem);
                NotifyOfPropertyChange(() => CanRemove);
                NotifyOfPropertyChange(() => CanMoveDown);
                NotifyOfPropertyChange(() => CanMoveUp);
                NotifyOfPropertyChange(() => CanEdit);
                NotifyOfPropertyChange(() => CanShowDetails);
            }
        }

        public MenuCommand AddMouseItemCommand { get; set; }
        public MenuCommand AddKeyItemCommand { get; set; }
        public MenuCommand AddStringListItemCommand { get; set; }
        public MenuCommand ExitCommand { get; set; }
        public MenuCommand ResetCountersCommand { get; set; }


        public ShellViewModel() : this(new WindowManager())
        {
        }


        public ShellViewModel(IWindowManager windowManager)
        {
            this.windowManager = windowManager;
            this.keyboardHook = new KeyboardHook();
            AddKeyItemCommand = new MenuCommand(this, new KeyGesture(Key.D, ModifierKeys.Control));
            AddMouseItemCommand = new MenuCommand(this, new KeyGesture(Key.F, ModifierKeys.Control));
            AddStringListItemCommand = new MenuCommand(this, new KeyGesture(Key.G, ModifierKeys.Control));
            ExitCommand = new MenuCommand(this, new KeyGesture(Key.Q, ModifierKeys.Control));
            ResetCountersCommand = new MenuCommand(this, new KeyGesture(Key.R, ModifierKeys.Control));
            Configuration = new Configuration()
            {
                Items = new BindableCollection<Entry>(),
                ProcessName = "notepad",
                Shortcut = new KeyShortcut() {Key = Key.F5},
                Interval = 250
            };
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            var s = GetView() as Window;
            s.CommandBindings.Add(new CommandBinding(AddKeyItemCommand, (sender, args) => Open(CmdTypes.Key),
                CanExecuteAlwaysTrue));
            s.CommandBindings.Add(new CommandBinding(AddMouseItemCommand, (sender, args) => Open(CmdTypes.MouseClick),
                CanExecuteAlwaysTrue));
            s.CommandBindings.Add(new CommandBinding(AddStringListItemCommand,
                (sender, args) => Open(CmdTypes.StringList),
                CanExecuteAlwaysTrue));
            s.CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, SaveConfiguration,
                CanSaveConfiguration));
            s.CommandBindings.Add(new CommandBinding(ExitCommand, (sender, args) => TryClose(),
                CanExecuteAlwaysTrue));
            s.CommandBindings.Add(new CommandBinding(ResetCountersCommand, ResetCounters,
                (sender, args) => args.CanExecute = SelectedItem != null));
            if (File.Exists(NameOfConfigFile))
            {
                try
                {
                    Configuration = Configuration.Read(NameOfConfigFile);
                    Configuration.GlobalShortcutEnabled = false;
                    NotifyOfPropertyChange(() => Items);
                    NotifyOfPropertyChange(() => IntervalValue);
                    NotifyOfPropertyChange(() => ProcessName);
                    NotifyOfPropertyChange(() => GlobalShortcutEnabled);
                }
                catch (Exception e)
                {
                    Util.MsgErr("Error while reading configuration:\n" + e);
                }
            }
            BotManager = new BotManager(Configuration, LimitToActiveWindow);
//            BotManager.ItemAdded += entry => { Items.Add(entry); };
//            BotManager.ItemRemoved += (i, entry) => { Items.Remove(entry); };
            BotManager.Called += () =>
            {
                this.RunOnUi(() =>
                {
                    Processing = processing[counter];
                    counter++;
                    if (counter == 4)
                        counter = 0;
                }, DispatcherPriority.Background);
            };
            BotManager.Started += () =>
            {
                Status = WorkingLabel;
                StatusColor = workingBrush;
                ApplicationCommands.Save.Execute(null, null);
                NotifyOfPropertyChange(() => CanStart);
                NotifyOfPropertyChange(() => CanStop);
            };
            BotManager.Stopped += () =>
            {
                Status = NotWorkingLabel;
                StatusColor = notWorkingBrush;
                NotifyOfPropertyChange(() => CanStart);
                NotifyOfPropertyChange(() => CanStop);
            };
            Console.WriteLine("MainThread: " + Thread.CurrentThread.ManagedThreadId);
        }

        private void CanExecuteAlwaysTrue(object o, CanExecuteRoutedEventArgs canExecuteRoutedEventArgs)
        {
            canExecuteRoutedEventArgs.CanExecute = true;
        }

        private void CanSaveConfiguration(object o, CanExecuteRoutedEventArgs canExecuteRoutedEventArgs)
        {
            canExecuteRoutedEventArgs.CanExecute = Items.Count > 0;
        }

        private void SaveConfiguration(object o, ExecutedRoutedEventArgs executedRoutedEventArgs)
        {
            Configuration.Save(NameOfConfigFile);
        }


        public void Add(Button btn, ContextMenu cm)
        {
//            var s = this.GetView().FindResource(ResourceAddMenu) as ContextMenu;
            cm.PlacementTarget = btn;
            cm.IsOpen = true;
        }


        public void Remove()
        {
            Items.Remove(SelectedItem);
        }

        public bool CanRemove => SelectedItem != null;

        public bool CanStart => !BotManager.IsWorking;

        public bool CanStop => BotManager.IsWorking;

        public void Start()
        {
            BotManager.Start(ProcessName, Convert.ToInt32(IntervalValue));
        }

        public void Stop()
        {
            BotManager.Stop();
        }

        public void MoveUp()
        {
            Util.Swap(Items, SelectedItem, SelectedIndex - 1);
        }

        public void MoveDown()
        {
            Util.Swap(Items, SelectedItem, SelectedIndex + 1);
        }

        public bool CanMoveUp => SelectedIndex > 0 && SelectedItem != null;

        public bool CanMoveDown => SelectedIndex < Items.Count - 1 && SelectedItem != null;

        public bool CanEdit => SelectedItem != null;
        public bool CanShowDetails => SelectedItem != null;

        public void Edit()
        {
            //; [Event MouseRightButtonDown] = [Action ShowDetails($eventArgs)]
            var add = new AddItemViewModel(SelectedItem, entry =>
            {
                Items.Refresh();
//                NotifyOfPropertyChange(() => Items);
            });
            windowManager.ShowDialog(add);
        }

        public void DoubleClick(MouseButtonEventArgs e)
        {
            if (SelectedItem != null)
            {
                Edit();
            }
        }


        public void Open(CmdTypes cmd)
        {
            var add = new AddItemViewModel(cmd, entry => { Items.Add(entry); });
            windowManager.ShowDialog(add);
        }

        public void FilterIntervalValue(TextCompositionEventArgs args)
        {
            args.Handled = Util.IsTextNumber(args.Text);
        }

        public void ShowDetails()
        {
            if (SelectedItem != null)
            {
                var d = new DetailsViewModel(Util.ToJson(SelectedItem));
                windowManager.ShowDialog(d);
            }
        }

        public void ShortcutChange()
        {
            var v = new KeyInputViewModel(keyShortcut =>
            {
                Configuration.Shortcut = keyShortcut;
                keyboardHook.UnregisterAll();
                keyboardHook.Register(Configuration.Shortcut, SwapRunningState);
                NotifyOfPropertyChange(() => Shortcut);
                if (GlobalShortcutEnabled)
                {
                    keyboardHook.Open();
                }
                // IsEnabled="{Binding ElementName=GlobalShortcutEnabled, Path=IsChecked}"
            });
            keyboardHook.Close();
            windowManager.ShowDialog(v);
        }

        private void ResetCounters(object sender, ExecutedRoutedEventArgs args)
        {
            var s = Items;
            foreach (var item in Items)
            {
                if (item.IsSelected && item.CmdType == CmdTypes.StringList)
                {
                    item.StringListData.ListItemNumer = 0;
                    item.StringListData.ChunkIndex = 0;
                }
            }
        }

        private void SwapRunningState()
        {
            if (BotManager.IsWorking)
            {
                Stop();
            }
            else
            {
                Start();
            }
        }


        private void ChangeGlobalShortcutState(bool enabled)
        {
            if (Configuration.Shortcut != null)
            {
                if (enabled)
                {
                    keyboardHook.Open();
                }
                else
                {
                    keyboardHook.Close();
                }
            }
        }
    }
}