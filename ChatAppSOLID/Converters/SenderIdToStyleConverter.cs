using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace ChatAppSOLID.Converters
{
    public class SenderIdToStyleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length == 2 && values[0] is string senderId && values[1] is string userId)
            {
                var returnStyle = senderId == userId
                    ? Application.Current.FindResource("UserMessageStyle") as Style
                    : Application.Current.FindResource("OtherMessageStyle") as Style;
                return returnStyle;
            }
            return null;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}