using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using Caliburn.Micro;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ShTaskerAndBot.Models;
using Action = System.Action;

namespace ShTaskerAndBot.Utils
{
    public static class Util
    {
        private static readonly JsonSerializerSettings jsonSerializerSettings = new JsonSerializerSettings
        {
//            TypeNameHandling = TypeNameHandling.Auto,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public static void MsgErr(string text)
        {
            MessageBox.Show(text, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public static void RunOnUi(this Screen screen, Action action)
        {
            Application.Current.Dispatcher.Invoke(action);
        }

        public static void RunOnUi(this Screen screen, Action action, DispatcherPriority priority)
        {
            Application.Current.Dispatcher.Invoke(action, priority);
        }

        public static string ToJson(object o)
        {
            return JsonConvert.SerializeObject(o, Formatting.Indented, jsonSerializerSettings);
        }

        public static T FromJson<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, jsonSerializerSettings);
        }

        public static void Swap<T>(IList<T> list, T a, int indexB)
        {
            int ia = list.IndexOf(a);
            list[ia] = list[indexB];
            list[indexB] = a;
        }
    }

    public class CmdTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CmdTypes t = (CmdTypes) value;
            string s;
            switch (t)
            {
                default:
                    s = "K";
                    break;
                case CmdTypes.Mouse:
                    s = "M";
                    break;
                case CmdTypes.StringList:
                    s = "S";
                    break;
            }
            return s;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
