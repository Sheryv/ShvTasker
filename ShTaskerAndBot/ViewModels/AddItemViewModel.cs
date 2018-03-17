using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using ShTaskerAndBot.Models;
using ShTaskerAndBot.Views;

namespace ShTaskerAndBot.ViewModels
{
    public class AddItemViewModel : Conductor<IAddItemPage>
    {
        private readonly CmdTypes cmdType;
        private readonly IAddItemPage page;
        private readonly Action<Entry> onAdded;

        public string EntryName { get; set; }

        public AddItemViewModel(CmdTypes cmdType, Action<Entry> onAdded)
        {
            this.cmdType = cmdType;
            this.onAdded = onAdded;
            this.page = GetPage(cmdType);
            EntryName = $"Cmd_{Entry.Counter:00}";
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            ActivateItem(page);
        }

        public void Add()
        {
            var e = page.FetchEntry();
            if (e != null)
            {
                e.CmdType = cmdType;
                e.Name = EntryName;
                onAdded.Invoke(e);
                TryClose();
            }
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
    }

    public interface IAddItemPage
    {
        Entry FetchEntry();
    }
}