using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using ShTaskerAndBot.Models;

namespace ShTaskerAndBot.ViewModels
{
    public class KeyItemViewModel : Screen, IAddItemPage
    {
        public string Keys
        {
            get => keys;
            set
            {
                keys = value; 
                NotifyOfPropertyChange(() => Keys);
            }
        }

        public Entry FetchEntry()
        {
            if (entry == null)
            {
                entry = new Entry();
            }
            entry.Keys = Keys;
            return entry;
        }

        private Entry entry;
        private string keys;

        public void Init(Entry e)
        {
            this.entry = e;
            Keys = e.Keys;
        }
    }
}