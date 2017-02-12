using System;
using System.Globalization;
using System.Net;
using System.Windows.Data;

namespace Monitor
{
    public class IpToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                if (value is IPAddress)
                {
                    return value.ToString();
                }
                return value.ToString();
            }
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
