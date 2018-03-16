using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ShTaskerAndBot.Models
{
    public class Entry
    {
        private string command = null;
        public static int Counter = 0;

        [JsonIgnore] public StringsListData StringListData;

        public string Path { get; set; }

        public string Name { get; set; }

        [JsonIgnore]
        public string Command
        {
            get
            {
                if (command != null)
                    return command;
                command = Keys;
                if (CmdType == CmdTypes.Mouse)
                    command= ">>Mouse: " + MouseBtn;
                if (CmdType == CmdTypes.StringList)
                    command= ">>String: " + Path;
                return command;
            }
        }

        public bool IsEnabled { get; set; } = true;

        public string Keys { get; set; }

        public int Period { get; set; }

        public MouseBtns MouseBtn { get; set; }

        public int Id { get; }

        public CmdTypes CmdType { get; set; }

        public bool Repeat { get; set; }

        public string Seperator { get; set; }

        public Entry(CmdTypes cmdType, string keys, int period, MouseBtns btn, bool repeat, string seperator)
        {
            Id = Counter;
            Counter++;
            this.CmdType = cmdType;
            this.Keys = keys;
            
            this.Period = period;
            this.MouseBtn = btn;
            this.Repeat = repeat;
            this.Seperator = seperator;
        }

        public Entry(string keys) : this(CmdTypes.Key, keys, 0, MouseBtns.Left, false, null)
        {

        }

        public Entry(MouseBtns btn) : this(CmdTypes.Mouse, null, 0, btn, false, null)
        {

        }

        public Entry()
        {
            Id = Counter;
            Counter++;
        }

        public override string ToString()
        {
            return $"{Id}. [time {Period}]: {Command}";
        }
    }

    public enum MouseBtns
    {
        Left,
        Right
    }

    public enum CmdTypes
    {
        Key=0,
        Mouse=1,
        StringList=2
    }

    public class StringsListData
    {
        public int ListItemNumer = 0;
        public string[] Chunks;
        public int ChunkIndex = 0;
        public string PreviousPart;
    }
}
