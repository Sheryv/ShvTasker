using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Newtonsoft.Json;
using ShTaskerAndBot.Utils;

namespace ShTaskerAndBot.Models
{
    public class Configuration
    {
        private BindableCollection<Entry> items;
        private string processName;
        private int interval;

        public BindableCollection<Entry> Items
        {
            get => items;
            set => items = value;
        }

        public string ProcessName
        {
            get => processName;
            set => processName = value;
        }

        public int Interval
        {
            get => interval;
            set => interval = value;
        }

        public override string ToString()
        {
            return Util.ToJson(this);
        }

        public void Save(string file)
        {
            var s = ToString();
            File.WriteAllText(file, s);
        }

        public static Configuration Read(string file)
        {
            string json = File.ReadAllText(file);
            var config = Util.FromJson<Configuration>(json);
            var max = config.items.Max(entry => entry.Id);
            Entry.Counter = max + 1;
            return config;
        }
    }
}
