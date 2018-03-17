using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;

namespace ShTaskerAndBot.ViewModels
{
    public class DetailsViewModel : Screen
    {
        public DetailsViewModel(string json)
        {
            Details = json;
        }

        public string Details { get; set; }

    }
}
