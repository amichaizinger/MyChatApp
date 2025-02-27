using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using ChatAppSOLID.Models;

namespace ChatAppSOLID.Converters
{
    public class SenderIdToStyleConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Guid senderId)
            {
                // Get the current user's ID from the Chat class
                Guid currentUserId = Chat._currentUserId; // Access static property (or use a service if more complex)

                bool isSentByCurrentUser = senderId == currentUserId;

                switch (parameter?.ToString())
                {
                    case "Alignment":
                        return isSentByCurrentUser ? HorizontalAlignment.Right : HorizontalAlignment.Left;
                    case "Background":
                        return isSentByCurrentUser ? Brushes.LightBlue : Brushes.White;
                    case "Foreground":
                        return isSentByCurrentUser ? Brushes.White : Brushes.Black;
                    case "TimeForeground":
                        return isSentByCurrentUser ? Brushes.White : Brushes.Gray;
                    default:
                        throw new ArgumentException("Invalid parameter for SenderIdToStyleConverter. Use 'Alignment', 'Background', 'Foreground', or 'TimeForeground'.");
                }
            }
            throw new ArgumentException("Value must be a string (SenderId).");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}