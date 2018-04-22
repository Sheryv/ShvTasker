using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Caliburn.Micro;
using ShvTasker.Models;

namespace ShvTasker.ViewModels
{
    public class KeyInputViewModel : Screen
    {
        private readonly Action<KeyShortcut> onKeysAction;
        

        public string Shortcut
        {
            get => "Press any keys combination";
        }


        public KeyInputViewModel(Action<KeyShortcut> onKeysAction)
        {
            this.onKeysAction = onKeysAction;
        }

        public void Change(KeyEventArgs e)
        {
            if (KeyShortcut.IsModifier(e.Key))
            {
                e.Handled = false;
                return;
            }
//            Console.WriteLine(">>: " + e.Key);
            var shortcut = new KeyShortcut()
            {
                Key = e.Key,
                Modifiers = Keyboard.Modifiers
            };
            onKeysAction.Invoke(shortcut);
            TryClose();
        }
    }
}
