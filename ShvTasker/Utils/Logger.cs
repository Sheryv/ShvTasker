using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShvTasker.ViewModels;

namespace ShvTasker.Utils
{
    public class Logger
    {
        private readonly Action<string> onTextWrite;
        private readonly object app;

        public Logger(Action<string> writeText, object app)
        {
            onTextWrite = writeText;
            this.app = app;
        }


        public void Log(string msg)
        {
            Log(msg, null);
        }

        public void E(string msg)
        {
            Log("ERR: " + msg);
        }

//        public void E(string msg, BaseOperation op)
//        {
//            Log($"ERR: {msg} <{op.Name} {{{op.Id}}}>");
//        }

        public void E(Exception e)
        {
            Log(e.ToString());
        }

        public void D(string msg)
        {
            Log(msg);
        }
//
//        public void D(string msg, BaseOperation op)
//        {
//            Log(msg + " <" + op.Name + " {" + op.Id + "}>");
//        }

//        public void DirectWriteLine(string msg, BaseOperation op)
//        {
//            if (op != null)
//            {
//                onTextWrite.Invoke("\n" + op.Id + " >>" + msg);
//            }
//            else
//            {
//                onTextWrite.Invoke("\n>>" + msg);
//            }
//        }

        public void Log()
        {
            Log("\n", null);
        }

        public void Log(string msg, object sender)
        {
            //for tests
            if (app == null || onTextWrite == null)
            {
                if (sender == null)
                {
                    Console.WriteLine(msg);
                }
                else
                {
                    Console.WriteLine(sender + "> " + msg);
                }
                return;
            }


            if (sender == null)
            {
                onTextWrite.Invoke($"\n[{DateTime.Now.ToLongTimeString()}]: {msg}");
            }
            else
            {
                string s;
                if (sender is Type type)
                {
                    s = type.Name;
                }
                else
                {
                    s = sender.GetType().Name;
                }
                onTextWrite.Invoke($"\n[{DateTime.Now.ToLongTimeString()}] {s}: {msg}");
            }
        }
    }

    public static class L
    {
        public static void Log(string msg)
        {
            Bootstrapper.Log.Log(msg);
        }

        public static void E(string msg)
        {
            Bootstrapper.Log.E(msg);
        }

//        public static void E(string msg, BaseOperation op)
//        {
//            Bootstrapper.Log.E(msg);
//        }

        public static void E(Exception e)
        {
            Bootstrapper.Log.E(e.ToString());
        }

        public static void D(string msg)
        {
            Bootstrapper.Log.D(msg);
        }

//        public static void D(string msg, BaseOperation op)
//        {
//            Bootstrapper.Log.D(msg, op);
//        }
//
//        public static void DirectWrite(string msg, BaseOperation op)
//        {
//            Bootstrapper.Log.DirectWriteLine(msg, op);
//        }

        public static void Log()
        {
            Bootstrapper.Log.Log();
        }

        public static void Log(string msg, object sender)
        {
            Bootstrapper.Log.Log(msg, sender);
        }
    }
}
