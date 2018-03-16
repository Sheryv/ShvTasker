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
        public bool LeftBtn { get; set; } = true;

        public Entry FetchEntry()
        {
            return new Entry()
            {
                MouseBtn = LeftBtn ? MouseBtns.Left : MouseBtns.Right
            };
        }
    }
}