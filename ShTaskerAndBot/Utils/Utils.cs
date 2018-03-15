using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using ShTaskerAndBot.Models;

namespace ShTaskerAndBot.Utils
{
    class Utils
    {
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
