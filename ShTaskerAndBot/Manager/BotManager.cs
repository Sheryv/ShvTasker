using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using AutoIt;
using ShTaskerAndBot.Models;
using ShTaskerAndBot.Views;
using MessageBox = System.Windows.MessageBox;
using Timer = System.Timers.Timer;

namespace ShTaskerAndBot.Manager
{
    public class BotManager
    {
        private const int DefaultPeriod = 100;


        private const string LeftMouseBtn = "left";
        private const string RightMouseBtn = "right";

        public bool IsWorking { get; private set; }
        public event Action<Entry> ItemAdded;
        public event Action<Entry> Started;
        public event Action<Entry> Stopped;
        public event Action<int, Entry> ItemRemoved;
        public event Action Called;

        private readonly Timer timer;
        private readonly MainWindow w;
        public List<Entry> Items { get; private set; } = new List<Entry>();
        private IntPtr handle;
        private Process process;


        public BotManager(MainWindow w)
        {
            this.w = w;
            timer = new Timer(DefaultPeriod);
            timer.Elapsed += Tick;
        }

        private void Tick(object sender, EventArgs args)
        {
            if (!process.HasExited)
            {
                handle = process.MainWindowHandle;
                if (handle != null && handle != IntPtr.Zero)
                {


                    //                    if (Items[0].IsMouse)
                    //                    {
                    //                        KeySender.PostMeth(handle);
                    //                    }
                    //                    else
                    //                    {
                    //                        KeySender.SendMeth();
                    //                    }



                    var winActive = AutoItX.WinActive(handle);
                    if (winActive == 1)
                    {
                        Console.WriteLine(@"Activ: " + winActive);
                        foreach (var item in Items)
                        {
                           // KeySender.SendMeth(item.Keys);
                           SendKeys.SendWait(item.Keys);
                            //                            if (item.IsMouse)
                            //                            {
                            //                                AutoItX.MouseClick(item.Btn == MouseBtn.Left ? LeftMouseBtn : RightMouseBtn);
                            //                            }
                            //                            else
                            //                            {
                            //                                AutoItX.Send(item.Keys);
                            //                            }

                        }
                        //                        AutoItX.Send("T");
                        Called?.Invoke();
                    }
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

        public void LoadConfig(Configuration c)
        {
            c.Items.ForEach(entry =>
            {
                Items.Add(entry);
                ItemAdded?.Invoke(entry);
            });
        }

        public bool Start(string processName, int period = DefaultPeriod)
        {
            var procId = Process.GetProcesses();
            foreach (var p in procId)
            {
                if (p.ProcessName.ToLower().Contains(processName.ToLower()))
                {
                    process = p;
                    Console.WriteLine(@"P: " + p);
                    timer.Interval = period;
                    timer.Start();
                    IsWorking = true;
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
        }

        public Entry Add(string keys)
        {
            var entry = new Entry(keys);
            Items.Add(entry);
            ItemAdded?.Invoke(entry);
            return entry;
        }

        public Entry Add(MouseBtn btn)
        {
            var entry = new Entry(btn);
            Items.Add(entry);
            ItemAdded?.Invoke(entry);
            return entry;
        }

        public void Remove(int id)
        {
            for (var i = 0; i < Items.Count; i++)
            {
                if (Items[i].Id == id)
                {
                    ItemRemoved?.Invoke(id, Items[i]);
                    Items.RemoveAt(i);
                    return;
                }
            }
        }
    }
}
