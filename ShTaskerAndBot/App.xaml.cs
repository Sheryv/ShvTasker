using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Reflection;
using System.Windows;
using System.ComponentModel;
using ShTaskerAndBot.Utils;
using ShTaskerAndBot.ViewModels;
using ShTaskerAndBot.Views;

namespace ShTaskerAndBot
{
    public partial class App : Application
    {
//        private static readonly ILog Log = log4net.LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static MainWindow app;

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            Logger Log = new Logger(null, null);
//            Log.D("Application Startup");

            // For catching Global uncaught exception
            AppDomain currentDomain = AppDomain.CurrentDomain;
            currentDomain.UnhandledException += new UnhandledExceptionEventHandler(UnhandledExceptionOccured);

            app = new MainWindow();
            var context = new MainViewModel();
            context.Log = Log;
            app.DataContext = context;
            app.Show();
            LogMachineDetails();
            Log.D("Starting App");

//            if (e.Args.Length == 1) //make sure an argument is passed
//            {
//                Log.D("File type association: " + e.Args[0]);
//                FileInfo file = new FileInfo(e.Args[0]);
//                if (file.Exists) //make sure it's actually a file
//                {
//                    // Here, add you own code
//                    // ((MainViewModel)app.DataContext).OpenFile(file.FullName);
//                }
//            }
        }

        static void UnhandledExceptionOccured(object sender, UnhandledExceptionEventArgs args)
        {
            // Here change path to the log.txt file
            var path = "D:\\plik.txt";

            // Show a message before closing application
            Exception e = (Exception)args.ExceptionObject;
            MessageBox.Show(
                "Oops, something went wrong and the application must close."+
                "If the problem persist, please contact SheryvL.\n"+e.Message+"\n"+e.StackTrace,
                "Unhandled Error",
                MessageBoxButton.OK);

        }

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
