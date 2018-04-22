using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using Newtonsoft.Json;
using ShvTasker.Utils;

namespace ShvTasker.Models
{
    public class Configuration
    {
        private BindableCollection<Entry> items;

        public BindableCollection<Entry> Items
        {
            get => items;
            set => items = value;
        }

        public bool GlobalShortcutEnabled { get; set; }

        public bool UseBuiltinSendWait { get; set; } = true;

        public KeyShortcut Shortcut { get; set; }   

        public string ProcessName { get; set; }

        public int Interval { get; set; }

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
