using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShTaskerAndBot.Models
{
    public class Entry
    {
        private static int counter = 0;
        private string keys;
        private int period;
        private MouseBtn btn;
        private int id;
        private CmdTypes cmdType;
        private bool repeat;
        private string seperator;
        private string name;
        private string command = null;

        public string Path { get; set; }

        public string Name
        {
            get => name;
            set
            {
                name = value;
            }
        }

        public string Command
        {
            get
            {
                if (command != null)
                    return command;
                command = Keys;
                if (CmdType == CmdTypes.Mouse)
                    command= ">>Mouse: " + Btn;
                if (CmdType == CmdTypes.StringList)
                    command= ">>String: " + Path;
                return command;
            }
        }


        public string Keys
        {
            get => keys;
            set => keys = value;
        }

        public int Period
        {
            get => period;
            set => period = value;
        }

        public MouseBtn Btn
        {
            get => btn;
            set => btn = value;
        }

        public int Id
        {
            get => id;
        }

        public CmdTypes CmdType
        {
            get => cmdType;
            set => cmdType = value;
        }

        public bool Repeat
        {
            get => repeat;
            set => repeat = value;
        }


        public string Seperator
        {
            get => seperator;
            set => seperator = value;
        }

        public Entry(CmdTypes cmdType, string keys, int period, MouseBtn btn, bool repeat, string seperator)
        {
            id = counter;
            counter++;
            this.cmdType = cmdType;
            this.keys = keys;
            if (Keys == null)
            {
                this.keys = "[Mouse]: " + Btn;
            }
            this.period = period;
            this.btn = btn;
            this.repeat = repeat;
            this.seperator = seperator;
        }

        public Entry(string keys) : this(CmdTypes.Key, keys, 0, MouseBtn.Left, false, null)
        {

        }

        public Entry(MouseBtn btn) : this(CmdTypes.Mouse, null, 0, btn, false, null)
        {

        }

        public Entry()
        {
            id = counter;
            counter++;
        }

        public override string ToString()
        {
            return $"{Id}. [time {Period}]: {Command}";
        }
    }

    public enum MouseBtn
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
}
