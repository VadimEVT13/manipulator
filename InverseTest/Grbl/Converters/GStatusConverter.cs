using InverseTest.Grbl.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace InverseTest.Grbl.Converters
{
    public class GStatusToStringConverter : IValueConverter
    {
        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is GStatus)
            {
                switch (value)
                {
                    case GStatus.IDLE:
                        return "Idle";
                    case GStatus.RUN:
                        return "Run";
                    case GStatus.HOLD:
                        return "Hold";
                    case GStatus.DOOR:
                        return "Door";
                    case GStatus.HOME:
                        return "Home";
                    case GStatus.CHECK:
                        return "Check";
                    case GStatus.DISCONNECT:
                        return "Disconnect";
                    default:
                        return "Alarm";
                }
            }
            return "Alarm";
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value.ToString().ToLower())
            {
                case "idle":
                    return GStatus.IDLE;
                case "run":
                    return GStatus.RUN;
                case "hold":
                    return GStatus.HOLD;
                case "door":
                    return GStatus.DOOR;
                case "home":
                    return GStatus.HOME;
                case "check":
                    return GStatus.CHECK;
                case "disconnect":
                    return GStatus.DISCONNECT;
                default:
                    return GStatus.ALARM;
            }
        }
    }
}
