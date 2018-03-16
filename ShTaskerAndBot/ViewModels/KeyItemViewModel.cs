using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShTaskerAndBot.Models;

namespace ShTaskerAndBot.ViewModels
{
    public class KeyItemViewModel : IAddItemPage
    {
        public string Keys { get; set; }

        public Entry FetchEntry()
        {
            return new Entry()
            {
                Keys = Keys,
            };
        }

    }
}
