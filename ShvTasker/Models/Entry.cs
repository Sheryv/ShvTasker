using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ShvTasker.Models
{
    public class Entry
    {
        private string command = null;
        public static int Counter = 0;

        [JsonIgnore] public StringsListData StringListData;

        [JsonIgnore]
        public bool IsSelected { get; set; }

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
                if (CmdType == CmdTypes.MouseClick)
                    command = MouseBtn.ToString();
                if (CmdType == CmdTypes.StringList)
                    command = Path;
                return command;
            }
        }

        public bool IsEnabled { get; set; } = true;

        public int LoopCount { get; set; } = 0;

        public int LoopInterval { get; set; } = 0;

        public int InitialDelay { get; set; } = 0;

        public string Keys { get; set; }

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

            this.MouseBtn = btn;
            this.Repeat = repeat;
            this.Seperator = seperator;
        }

        public Entry(string keys) : this(CmdTypes.Key, keys, 0, MouseBtns.Left, false, null)
        {
        }

        public Entry(MouseBtns btn) : this(CmdTypes.MouseClick, null, 0, btn, false, null)
        {
        }

        public Entry()
        {
            Id = Counter;
            Counter++;
        }

        public override string ToString()
        {
            string s = $"{Id}.> {Command} [Loop:{LoopCount}, Interval:{LoopInterval}, Delay:{InitialDelay}]";
            if (CmdType == CmdTypes.StringList)
            {
                s += $"[Repeat:{Repeat}]";
            }
            return s;
        }

        public void ResetCache()
        {
            command = null;
        }
    }

    public enum MouseBtns
    {
        Left,
        Right
    }

    public enum CmdTypes
    {
        Key = 0,
        MouseClick = 1,
        StringList = 2,
        MouseMove = 3
    }

    public class StringsListData
    {
        public int ListItemNumer = 0;
        public string[] Chunks;
        public int ChunkIndex = 0;
        public string PreviousPart;
    }
}