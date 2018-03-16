using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
                Configuration.Interval = Convert.ToInt32(value);
                NotifyOfPropertyChange(() => IntervalValue);
            }
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
            }
        }

        public MenuCommand AddMouseItemCommand { get; set; }
        public MenuCommand AddKeyItemCommand { get; set; }
        public MenuCommand AddStringListItemCommand { get; set; }
        public MenuCommand ExitCommand { get; set; }


        public ShellViewModel() : this(new WindowManager())
        {
        }


        public ShellViewModel(IWindowManager windowManager)
        {
            this.windowManager = windowManager;
            AddKeyItemCommand = new MenuCommand(this, new KeyGesture(Key.D, ModifierKeys.Control));
            AddMouseItemCommand = new MenuCommand(this, new KeyGesture(Key.F, ModifierKeys.Control));
            AddStringListItemCommand = new MenuCommand(this, new KeyGesture(Key.G, ModifierKeys.Control));
            ExitCommand = new MenuCommand(this, new KeyGesture(Key.Q, ModifierKeys.Control));
            Configuration = new Configuration()
            {
                Items = new BindableCollection<Entry>(),
                ProcessName = "notepad",
                Interval = 250
            };
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            var s = GetView() as Window;
            s.CommandBindings.Add(new CommandBinding(AddKeyItemCommand, (sender, args) => Open(CmdTypes.Key),
                CanExecuteAlwaysTrue));
            s.CommandBindings.Add(new CommandBinding(AddMouseItemCommand, (sender, args) => Open(CmdTypes.Mouse),
                CanExecuteAlwaysTrue));
            s.CommandBindings.Add(new CommandBinding(AddStringListItemCommand,
                (sender, args) => Open(CmdTypes.StringList),
                CanExecuteAlwaysTrue));
            s.CommandBindings.Add(new CommandBinding(ApplicationCommands.Save, SaveConfiguration,
                CanSaveConfiguration));
            s.CommandBindings.Add(new CommandBinding(ExitCommand, (sender, args) => TryClose(),
                CanExecuteAlwaysTrue));
            if (File.Exists(NameOfConfigFile))
            {
                try
                {
                    Configuration = Configuration.Read(NameOfConfigFile);
                    NotifyOfPropertyChange(() => Items);
                    NotifyOfPropertyChange(() => IntervalValue);
                    NotifyOfPropertyChange(() => ProcessName);
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

        public bool CanMoveDown => SelectedIndex < Items.Count-1 && SelectedItem != null;

        public void Open(CmdTypes cmd)
        {
            var add = new AddItemViewModel(cmd, entry => { Items.Add(entry); });
            windowManager.ShowDialog(add);
        }

        public void FilterIntervalValue(TextCompositionEventArgs args)
        {
            args.Handled = IsTextAllowed(args.Text);
        }

        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9]+"); 
            return regex.IsMatch(text);
        }
    }
}