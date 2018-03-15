using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ShTaskerAndBot.Models
{
    public class Configuration
    {
        private List<Entry> items;
        private string processName;
        private int interval;

        public List<Entry> Items
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
            return JsonConvert.SerializeObject(this);
        }

        public static Configuration Read(string file)
        {
            string json = File.ReadAllText(file);
            var config = JsonConvert.DeserializeObject<Configuration>(json);
            return config;
        }
    }
}
