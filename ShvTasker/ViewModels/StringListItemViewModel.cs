using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ShvTasker.Models;
using ShvTasker.Utils;

namespace ShvTasker.ViewModels
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
            if (entry == null)
            {
                entry = new Entry();
            }
            entry.Path = Path;
            entry.Seperator = Separator;
            entry.Repeat = IsRepeat;
            return entry;
        }


        private Entry entry;
        public void Init(Entry e)
        {
            this.entry = e;
            Path = e.Path;
            Separator = e.Seperator;
            IsRepeat = e.Repeat;
        }
    }
}