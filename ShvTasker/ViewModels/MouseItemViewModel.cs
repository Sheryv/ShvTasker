using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using ShvTasker.Models;

namespace ShvTasker.ViewModels
{
    public class MouseItemViewModel : Screen, IAddItemPage
    {
        public bool LeftBtn
        {
            get => leftBtn;
            set
            {
                leftBtn = value;
                NotifyOfPropertyChange(() => LeftBtn);
            }
        }

        public Entry FetchEntry()
        {
            if (entry == null)
            {
                entry = new Entry();
            }
            entry.MouseBtn = LeftBtn ? MouseBtns.Left : MouseBtns.Right;
            return entry;
        }

        private Entry entry;
        private bool leftBtn = true;

        public void Init(Entry e)
        {
            this.entry = e;
            LeftBtn = e.MouseBtn == MouseBtns.Left;
        }
    }
}