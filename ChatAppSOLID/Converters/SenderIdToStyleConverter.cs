using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ChatAppSOLID.Converters
{
    public class SenderIdToStyleConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values.Length != 2 || !(values[0] is Guid senderId) || !(values[1] is Guid userId) || parameter == null)
                return DependencyProperty.UnsetValue;

            bool isCurrentUser = senderId == userId;
            string param = parameter.ToString();

            switch (param)
            {
                case "Background":
                    return isCurrentUser
                        ? new SolidColorBrush(Color.FromRgb(220, 248, 198)) // #DCF8C6
                        : new SolidColorBrush(Colors.White);
                case "Alignment":
                    return isCurrentUser ? HorizontalAlignment.Right : HorizontalAlignment.Left;
                case "Foreground":
                    return new SolidColorBrush(Colors.Black);
                case "TimeForeground":
                    return new SolidColorBrush(Colors.Gray);
                default:
                    return DependencyProperty.UnsetValue;
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}