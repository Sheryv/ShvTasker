using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShTaskerAndBot.Models;

namespace ShTaskerAndBot.ViewModels
{
    public class MouseItemViewModel : IAddItemPage
    {

        private Action<Entry> onAdded;
        public void WithOnAdded(Action<Entry> onAdded)
        {
            this.onAdded = onAdded;
        }

    }
}
