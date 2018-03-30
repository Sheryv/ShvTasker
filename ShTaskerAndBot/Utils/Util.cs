using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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

        public static bool IsTextNumber(string text)
        {
            Regex regex = new Regex("[^0-9]+");
            return regex.IsMatch(text);
        }
    }
}
