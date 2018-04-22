using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Caliburn.Micro;
using ShvTasker.Views;
using ShvTasker.Models;
using ShvTasker.Utils;

namespace ShvTasker.ViewModels
{
    public class AddItemViewModel : Conductor<IAddItemPage>
    {

        private readonly CmdTypes cmdType;
        private readonly IAddItemPage page;
        private readonly Action<Entry> onAdded;
        private const string Default = "0";
        private Entry editedEntry;
        private string title = "Add new command";

        public string Title
        {
            get => title;
            set
            {
                title = value; 
                NotifyOfPropertyChange(() => Title);
            }
        }

        public bool IsEditing { get; set; } = false;

        public string LoopCount { get; set; } = "1";

        public string LoopInterval { get; set; } = Default;

        public string InitialDelay { get; set; } = "25";

        public string EntryName { get; set; }

        public AddItemViewModel(CmdTypes cmdType, Action<Entry> onAdded)
        {
            this.cmdType = cmdType;
            this.onAdded = onAdded;
            this.page = GetPage(cmdType);
            EntryName = $"Cmd_{Entry.Counter:00}";
        }

        public AddItemViewModel(Entry edit, Action<Entry> onAdded)
        {
            IsEditing = true;
            editedEntry = edit;
            EntryName = edit.Name;
            LoopCount = edit.LoopCount.ToString();
            LoopInterval = edit.LoopInterval.ToString();
            InitialDelay = edit.InitialDelay.ToString();
            this.cmdType = edit.CmdType;
            this.onAdded = onAdded;
            this.page = GetPage(cmdType);
            page.Init(edit);
        }

        public AddItemViewModel()
        {
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            ActivateItem(page);
        }

        public bool CanAdd => !IsEditing;

        public void Edit()
        {
            Title = "Edit command";
            var e = LoadEntry();
            TryClose();
        }

        public void Add()
        {
            var e = LoadEntry();
            TryClose();
        }

        private Entry LoadEntry()
        {
            var e = page.FetchEntry();
            if (e != null)
            {
                e.Name = EntryName;
                bool err = false;
                int l = string.IsNullOrWhiteSpace(LoopCount) || LoopCount == "" ? 1 : Convert.ToInt32(LoopCount);
                if (l == 0)
                {
                    l = 1;
                    err = true;
                }
                e.CmdType = cmdType;
                e.LoopCount = l;
                e.InitialDelay = string.IsNullOrWhiteSpace(InitialDelay) || InitialDelay == ""
                    ? 0
                    : Convert.ToInt32(InitialDelay);
                e.LoopInterval = string.IsNullOrWhiteSpace(LoopInterval) || LoopInterval == ""
                    ? 0
                    : Convert.ToInt32(LoopInterval);

                onAdded.Invoke(e);
                if (err)
                    Util.MsgErr(
                        "Loop count was corrected to 1. It means number of execution and should be equal to or greater than 1.");
            }
            e.ResetCache();
            return e;
        }

        public static IAddItemPage GetPage(CmdTypes cmd)
        {
            IAddItemPage page;
            switch (cmd)
            {
                case CmdTypes.Key:
                    page = new KeyItemViewModel();
                    break;
                case CmdTypes.MouseClick:
                    page = new MouseItemViewModel();
                    break;
                default:
                    page = new StringListItemViewModel();
                    break;
            }
            return page;
        }


        public void FilterValue(TextCompositionEventArgs args)
        {
            args.Handled = Util.IsTextNumber(args.Text);
        }
    }

    public interface IAddItemPage
    {
        Entry FetchEntry();
        void Init(Entry entry);
    }
}