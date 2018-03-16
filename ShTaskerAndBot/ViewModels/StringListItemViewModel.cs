using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShTaskerAndBot.Models;
using ShTaskerAndBot.Utils;

namespace ShTaskerAndBot.ViewModels
{
    public class StringListItemViewModel : Caliburn.Micro.Screen, IAddItemPage
    {
        private string separator = "\n";

        private string path;

        public bool IsRepeat { get; set; } = true;

        public string Path
        {
            get => path;
            set
            {
                path = value;
                NotifyOfPropertyChange(() => Path);
            }
        }

        public string Separator
        {
            get => separator;
            set
            {
                separator = value;
                NotifyOfPropertyChange(() => Separator);
                NotifyOfPropertyChange(() => SeparatorLabel);
            }
        }

        public string SeparatorLabel => $"Separator\n(chars: {Separator.Length})";


        public void ChangePath()
        {
            var d = new OpenFileDialog
            {
                Multiselect = false,
                InitialDirectory = Directory.GetCurrentDirectory()
            };
            var r = d.ShowDialog();
            if (r == DialogResult.OK)
            {
                Path = d.FileName;
            }
        }

        public Entry FetchEntry()
        {
            if (!File.Exists(Path))
            {
                Util.MsgErr("Incorrect file path!");
                return null;
            }
            if (string.IsNullOrEmpty(Separator))
            {
                Util.MsgErr("Separator cannot be empty");
                return null;
            }

            return new Entry()
            {
                Path = Path,
                Seperator = Separator,
                Repeat = IsRepeat
            };
        }

    }
}