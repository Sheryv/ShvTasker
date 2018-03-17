using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Win32.SafeHandles;
using ShTaskerAndBot.Models;
using KeyEventArgs = System.Windows.Forms.KeyEventArgs;
using KeyEventHandler = System.Windows.Forms.KeyEventHandler;

namespace ShTaskerAndBot.Manager
{
    class KeyboardHook
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private readonly LowLevelKeyboardProc proc;
        private static IntPtr hookId = IntPtr.Zero;
        private bool working = false;
        private List<Record> records;
//        private Action<KeyShortcut> oneTimeKeys;

        public event Action<KeyShortcut> KeyDown;
        public event Action<bool> HookStateChanged;

//        public static int Key = 0;
//        public static int Flag = 0;

        public static Microsoft.Win32.SaveFileDialog dlgSave = new Microsoft.Win32.SaveFileDialog();
        // static StreamWriter writer;

        public KeyboardHook()
        {
            proc = HookCallback;
            records = new List<Record>();
        }

        public void Open()
        {
            if (working)
                return;
            hookId = SetHook(proc);
            HookStateChanged?.Invoke(true);
            working = true;
        }

//        public void OpenForOneShortcut(Action<KeyShortcut> onKeysAction)
//        {
//            oneTimeKeys = onKeysAction;
//            Open();
//        }

        public void Close()
        {
            if (!working)
                return;
            UnhookWindowsHookEx(hookId);
            HookStateChanged?.Invoke(false);
            working = false;
            //writer.Close();
        }

        public void Register(KeyShortcut shortcut, Action action)
        {
            records.Add(new Record(shortcut, action));
        }

        public void Unregister(KeyShortcut shortcut)
        {
            var s = records.FirstOrDefault(record => record.shortcut == shortcut);
            if (s != null)
            {
                records.Remove(s);
            }
        }

        public void UnregisterAll()
        {
            records.Clear();
        }


        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(
            int nCode, IntPtr wParam, IntPtr lParam);

        private IntPtr HookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0) return CallNextHookEx(hookId, nCode, wParam, lParam);

            if (wParam == (IntPtr) WM_KEYDOWN)
            {
                var vkCode = Marshal.ReadInt32(lParam);
                var k = KeyShortcut.FromVirtualKey(vkCode);
                var m = Keyboard.Modifiers;
//                if (!KeyShortcut.IsModifier(k.Key))
//                {
//
//                }
                KeyDown?.Invoke(new KeyShortcut {Key = k, Modifiers = m});
//                if (oneTimeKeys != null)
//                {
//                    if (!KeyShortcut.IsModifier(k))
//                    {
//                        oneTimeKeys.Invoke(new KeyShortcut() {Key = k, Modifiers = m});
//                        oneTimeKeys = null;
//                        Close();
//                    }
//                    return CallNextHookEx(hookId, nCode, wParam, lParam);
//                }

                foreach (var rec in records)
                {
                    if (rec.shortcut.Key == k && rec.shortcut.Modifiers == m)
                    {
                        rec.action.Invoke();
                    }
                }
                Console.WriteLine($"{vkCode}[{m}]: {k}|{Thread.CurrentThread.ManagedThreadId}");
            }

//            if ((MainWindow.StartStopKey == (Keys) vkCode) && (MainWindow.Paused == false) &&
//                (wParam == (IntPtr) WM_KEYDOWN))
//            {
//                MainWindow.Paused = true;
//                Debug.WriteLine("Paused");
//                return CallNextHookEx(hookId, nCode, wParam, lParam);
//            }
//            if (MainWindow.StartStopKey == (Keys) vkCode && MainWindow.Paused && (wParam == (IntPtr) WM_KEYDOWN))
//            {
//                MainWindow.Paused = false;
//
//                return CallNextHookEx(hookId, nCode, wParam, lParam);
//            }


//            if (wParam == (IntPtr) WM_KEYDOWN && MainWindow.Paused == false)
//            {
//                string s = ((Keys) vkCode) + ",  ";
//                //konsola(s);
//
//                KeyDown(this, kea);
//
//            }
//            else if (wParam == (IntPtr) WM_KEYUP && MainWindow.Paused == false)
//            {
//                if ((Keys) vkCode == Keys.LMenu || (Keys) vkCode == Keys.RMenu)
//                {
//                    if (MainWindow.consoleEnabled)
//                        Console.Write(((Keys) vkCode).ToString() + " ");
//                    KeyDown(this, kea);
//                }
//            }
            return CallNextHookEx(hookId, nCode, wParam, lParam);
        }


        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);


        internal class Record
        {
            internal KeyShortcut shortcut;
            internal Action action;

            public Record(KeyShortcut shortcut, Action action)
            {
                this.shortcut = shortcut;
                this.action = action;
            }
        }
    }
}