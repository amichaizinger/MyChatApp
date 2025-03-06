using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace ChatAppSOLID.Converters
{
        public class SenderIdToStyleConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value is string senderId)
                {
                string currentUserId = (string)Application.Current.Resources["CurrentUserId"]; // Set this in App.xaml.cs or elsewhere
                    return senderId == currentUserId
                        ? Application.Current.FindResource("UserMessageStyle") as Style
                        : Application.Current.FindResource("OtherMessageStyle") as Style;
                }
                return null;
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
