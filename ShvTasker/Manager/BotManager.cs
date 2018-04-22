using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using AutoIt;
using ShvTasker.Views;
using MessageBox = System.Windows.MessageBox;
using Timer = System.Timers.Timer;
using System.Threading;
using ShvTasker.Models;

namespace ShvTasker.Manager
{
    public class BotManager
    {
        private const int DefaultPeriod = 100;
        private const int ReadChunkMaxSize = 20 * 1024;

        private const string LeftMouseBtn = "left";
        private const string RightMouseBtn = "right";

        public bool IsWorking { get; private set; }

//        public event Action<Entry> ItemAdded;
//        public event Action<int, Entry> ItemRemoved;
        public event Action Started;

        public event Action Stopped;
        public event Action Called;

        private readonly Timer timer;
        private IntPtr handle;
        private Process process;
        private readonly Configuration configuration;
        private readonly bool limitToActiveWindow;

        public BotManager(Configuration configuration, bool limitToActiveWindow)
        {
            this.configuration = configuration;
            this.limitToActiveWindow = limitToActiveWindow;
            timer = new Timer(DefaultPeriod);
            timer.Elapsed += Tick;
        }

        private void Tick(object sender, EventArgs args)
        {
            if (!limitToActiveWindow)
            {
                ExecuteAll(IntPtr.Zero);
                return;
            }

            if (!process.HasExited)
            {
                handle = process.MainWindowHandle;
                if (handle != null && handle != IntPtr.Zero)
                {
                    ExecuteAll(handle);
                }
                else
                {
                    timer.Stop();
                }
            }
            else
            {
                timer.Stop();
            }
        }

        private void ExecuteAll(IntPtr handle)
        {
            var list = new List<Entry>(configuration.Items);
            foreach (var item in list)
            {
                if (!IsWorking)
                    return;
                if (!item.IsEnabled)
                {
                    continue;
                }
                if (item.InitialDelay > 0)
                {
                    Thread.Sleep(item.InitialDelay);
                }
                if (!IsWorking)
                    return;
                int winActive = 0;
                for (int i = 0; i < item.LoopCount; i++)
                {
                    if (!IsWorking)
                        return;
                    winActive = AutoItX.WinActive(handle);
                    if (winActive == 1)
                    {
                        switch (item.CmdType)
                        {
                            case CmdTypes.Key:
                                ExecuteKeys(item);
                                break;
                            case CmdTypes.MouseClick:
                                ExecuteMouse(item);
                                break;
                            default:
                                ExecuteStringList(item);
                                break;
                        }
                    }
                    if (item.LoopInterval > 0 && i < item.LoopCount-1)
                    {
                        Thread.Sleep(item.LoopInterval);
                    }
                    if (!IsWorking)
                        return;
                }
                if (winActive == 1)
                    Called?.Invoke();
            }
        }

        public void ExecuteKeys(Entry entry)
        {
            //            SendKeys.SendWait(entry.Keys);
            if (configuration.UseBuiltinSendWait)
            {
                try
                {
                SendKeys.SendWait(entry.Keys);
                }
                catch
                {
                }
            }
            else
            {
                AutoItX.Send(entry.Keys);
            }
        }

        public void ExecuteMouse(Entry entry)
        {
            AutoItX.MouseClick(entry.MouseBtn == MouseBtns.Left ? LeftMouseBtn : RightMouseBtn);
        }

        public void ExecuteStringList(Entry entry)
        {
            if (entry.StringListData == null)
                entry.StringListData = new StringsListData();

            string toSend = null;
            if (entry.StringListData.Chunks == null)
            {
                try
                {
                    int done;
                    char[] b = new char[ReadChunkMaxSize];
                    using (var reader = new StreamReader(new FileStream(entry.Path, FileMode.Open)))
                    {
                        done = reader.ReadBlock(b, entry.StringListData.ChunkIndex, ReadChunkMaxSize);
                    }
                    string s = new string(b, 0, done);
                    entry.StringListData.Chunks = s.Split(entry.Seperator.ToCharArray());
                }
                catch (Exception e)
                {
                    MessageBox.Show("Error while reading specified text chunk from " + entry.Path + ". Details:\n" + e);
                    return;
                }
            }
            if (entry.StringListData.ListItemNumer >= entry.StringListData.Chunks.Length)
            {
                if (!entry.Repeat)
                {
                    Stop();
                    return;
                }
                entry.StringListData.ListItemNumer = 0;
            }

            toSend = entry.StringListData.Chunks[entry.StringListData.ListItemNumer];
            entry.StringListData.ListItemNumer++;
            if (toSend == "")
                return;
            if (configuration.UseBuiltinSendWait)
            {
                SendKeys.SendWait(toSend);
            }
            else
            {
                AutoItX.Send(toSend);
            }
        }


        public bool Start(string processName, int period = DefaultPeriod)
        {
            int time = 0;
            foreach (var item in configuration.Items)
            {
                if(!item.IsEnabled)
                    continue;
                time += item.InitialDelay;
                if (item.LoopCount > 0)
                {
                    time += (item.LoopCount - 1) * item.LoopInterval;
                }
            }
            if (time > configuration.Interval)
            {
                Utils.Util.MsgErr(
                    $"Duration of sequence cannot be longer than execution interval value! Current duration is " +
                    $"{time} and is too long by {(time - configuration.Interval)}\nRecommended value is {time + 10}");
                return false;
            }

            var procId = Process.GetProcesses();
            foreach (var p in procId)
            {
                if (p.ProcessName.ToLower().Contains(processName.ToLower()))
                {
                    process = p;
//                    Console.WriteLine(@"P: " + p);
                    timer.Interval = period;
                    timer.Start();
                    IsWorking = true;
                    Started?.Invoke();
                    return true;
                }
            }
            MessageBox.Show(@"Nie można odnaleźć procesu o nazwie zawierającej frazę: " + processName,
                @"Błąd", MessageBoxButton.OK);
            return false;
        }

        public void Stop()
        {
            IsWorking = false;
            timer.Stop();
            Stopped?.Invoke();
        }
    }
}