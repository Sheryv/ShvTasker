using System;
using System.Windows;
using System.Windows.Data;
using Caliburn.Micro;
using ShvTasker.Models;
using ShvTasker.Utils;
using ShvTasker.ViewModels;

namespace ShvTasker
{
    public class Bootstrapper : BootstrapperBase
    {

        public static Logger Log;
        public Bootstrapper()
        {
            Initialize();
        }

        protected override void OnStartup(object sender, StartupEventArgs e)
        {
            Log = new Logger(s =>
            {
                Console.WriteLine(s);
            }, Application.Current);
            DisplayRootViewFor<ShellViewModel>();
        }

        /* private static readonly IValueConverter CmdTypeToStringConverter = new CmdTypeToStringConverter();

         protected override void Configure()
         {

             var oldApplyConverterFunc = ConventionManager.ApplyValueConverter;

             ConventionManager.ApplyValueConverter = (binding, bindableProperty, property) => {
                 if (bindableProperty == UIElement.Opacity && typeof(CmdTypes).IsAssignableFrom(property.PropertyType))
                     //                                ^^^^^^^           ^^^^^^
                     //                             Property in XAML     Property in view-model
                     binding.Converter = CmdTypeToStringConverter;
                 //                  ^^^^^^^^^^^^^^^^^^^^^^^^^
                 //                 Our converter used here.

                 // else we use the default converter
                 else
                     oldApplyConverterFunc(binding, bindableProperty, property);

             };
         }*/


        private void LogMachineDetails()
        {
            var computer = new Microsoft.VisualBasic.Devices.ComputerInfo();

            string text = "OS: " + computer.OSPlatform + " v" + computer.OSVersion + Environment.NewLine +
                          computer.OSFullName + Environment.NewLine +
                          "RAM: " + computer.TotalPhysicalMemory.ToString() + Environment.NewLine +
                          "Language: " + computer.InstalledUICulture.EnglishName;
            L.D(text);
        }
    }
}