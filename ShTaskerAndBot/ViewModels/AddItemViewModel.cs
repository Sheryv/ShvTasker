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
        private IAddItemPage page;

        public AddItemViewModel(CmdTypes cmdType)
        {
            this.cmdType = cmdType;
            this.page = GetPage(cmdType);

        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            this.Activated += (sender, args) =>
            {
                Console.WriteLine("Wind active");
            };
            ActivateItem(page);
        }


        public void Add()
        {
            ActivateItem(page);
        }

        public static IAddItemPage GetPage(CmdTypes cmd)
        {
            IAddItemPage page;
            switch (cmd)
            {
                case CmdTypes.Key:
                    page=new KeyItemViewModel();
                    break;
                case CmdTypes.Mouse:
                    page=new MouseItemViewModel();
                    break;
                default:
                    page=new StringListItemViewModel();
                    break;
            }
            return page;
        }

    }

    public interface IAddItemPage
    {
        void WithOnAdded(Action<Entry> onAdded);
    }
}
