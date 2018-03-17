using System;
using System.Globalization;
using System.Windows.Data;
using ShTaskerAndBot.Models;

namespace ShTaskerAndBot.Converters
{
    public class CmdTypeToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            CmdTypes t = (CmdTypes) value;
            string s;
            switch (t)
            {
                default:
                    s = "Keys";
                    break;
                case CmdTypes.MouseClick:
                    s = "Click";
                    break;
                case CmdTypes.StringList:
                    s = "S-List";
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