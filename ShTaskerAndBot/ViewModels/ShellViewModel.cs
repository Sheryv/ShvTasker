using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Caliburn.Micro;
using ShTaskerAndBot.Models;
using ShTaskerAndBot.Utils;

namespace ShTaskerAndBot.ViewModels
{
    public class ShellViewModel : Screen
    {
        private readonly IWindowManager windowManager;
        private const string WorkingLabel = "Working";
        private const string NotWorkingLabel = "Not working";
        private const string Config = "config.json";
        private const string ResourceAddMenu = "CmAdd";
        private static readonly SolidColorBrush workingBrush = new SolidColorBrush(Colors.DarkGreen);
        private static readonly SolidColorBrush notWorkingBrush = new SolidColorBrush(Colors.DarkMagenta);
        private readonly string[] processing = new[] {"|", "/", "-", "\\"};

        public string Title { get; } = "ShTasker&Bot";
        public string ProcessName { get; set; } = "notepad";
        public string IntervalValue { get; set; } = 200.ToString();
        public string Processing { get; set; } = ".";
        public string Status { get; set; } = NotWorkingLabel;
        public SolidColorBrush StatusColor { get; set; } = notWorkingBrush;
        public BindableCollection<Entry> Items { get; set; }

        private Entry selectedItem;

        public Entry SelectedItem
        {
            get => selectedItem;
            set
            {
                selectedItem = value;
                NotifyOfPropertyChange(() => SelectedItem);
            }
        }

        public MenuCommand CmdBoo { get; set; }
        public MenuCommand AddMouseItemCommand { get; set; }
        public MenuCommand AddKeyItemCommand { get; set; }
        public MenuCommand AddStringListItemCommand { get; set; }


        public ShellViewModel() : this(new WindowManager())
        {
        }


        public ShellViewModel(IWindowManager windowManager)
        {
            this.windowManager = windowManager;
            CmdBoo = new MenuCommand(this, new KeyGesture(Key.T, ModifierKeys.Control));
            AddKeyItemCommand = new MenuCommand(this, new KeyGesture(Key.D, ModifierKeys.Control));
            AddMouseItemCommand = new MenuCommand(this, new KeyGesture(Key.F, ModifierKeys.Control));
            AddStringListItemCommand = new MenuCommand(this, new KeyGesture(Key.G, ModifierKeys.Control));
            CmdBoo = new MenuCommand(this, new KeyGesture(Key.A, ModifierKeys.Control));
            Items = new BindableCollection<Entry>();
            Items.Add(new Entry("keys"));
            Items.Add(new Entry("keys2"));
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            var s = GetView() as Window;
            s.CommandBindings.Add(new CommandBinding(CmdBoo, (sender, args) => Open2("s"),
                (sender, args) => args.CanExecute = true));
            s.CommandBindings.Add(new CommandBinding(AddKeyItemCommand, (sender, args) => Open(CmdTypes.Key),
                (sender, args) => args.CanExecute = true));
            s.CommandBindings.Add(new CommandBinding(AddMouseItemCommand, (sender, args) => Open(CmdTypes.Mouse),
                (sender, args) => args.CanExecute = true));
            s.CommandBindings.Add(new CommandBinding(AddStringListItemCommand, (sender, args) => Open(CmdTypes.StringList),
                (sender, args) => args.CanExecute = true));
        }

        public void Add(Button btn, ContextMenu cm)
        {
//            var s = this.GetView().FindResource(ResourceAddMenu) as ContextMenu;
//            s.PlacementTarget = sender as UIElement;
//            s.IsOpen = true;
            cm.PlacementTarget = btn;
            cm.IsOpen = true;
//            var add = new AddItemViewModel(CmdTypes.Key);
//            windowManager.ShowDialog(add);
        }


        public void Remove()
        {
            Console.WriteLine();
        }

        public void Start()
        {
        }

        public void Stop()
        {
        }

        public string Gesture { get; set; } =
            new KeyGesture(Key.A, ModifierKeys.Control).GetDisplayStringForCulture(CultureInfo.CurrentCulture);


        public void Open(CmdTypes cmd)
        {
            var add = new AddItemViewModel(cmd);
            windowManager.ShowDialog(add);
            Console.WriteLine("open");
        }

        public void Open2(object s)
        {
            Console.WriteLine("Open2 " + s);
        }

        public void Sds()
        {
            Console.WriteLine("sds");
        }

        public bool CanOpen(EventArgs args)
        {
            Console.WriteLine("caopen");
            return true;
        }

        public void FilterIntervalValue(TextCompositionEventArgs args)
        {
            args.Handled = IsTextAllowed(args.Text);
        }


        private static bool IsTextAllowed(string text)
        {
            Regex regex = new Regex("[^0-9]+"); //regex that matches disallowed text
            return regex.IsMatch(text);
        }
    }
}