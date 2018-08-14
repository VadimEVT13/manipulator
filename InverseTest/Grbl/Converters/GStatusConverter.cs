using InverseTest.Grbl.Models;
using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Windows.Data;

namespace InverseTest.Grbl.Converters
{
    public class GStatusToStringConverter : JsonConverter, IValueConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(string);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            switch ((string)reader.Value)
            {
                case "Idle":
                    return GState.IDLE;
                case "Run":
                    return GState.RUN;
                case "Hold":
                    return GState.HOLD;
                case "Home":
                    return GState.HOME;
                case "Alarm":
                    return GState.ALARM;
                case "Check":
                    return GState.CHECK;
                case "Door":
                    return GState.DOOR;
                default:
                    return GState.DISCONNECT;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            switch ((GState)value)
            {
                case GState.IDLE:
                    writer.WriteValue("Idle");
                    break;
                case GState.RUN:
                    writer.WriteValue("Run");
                    break;
                case GState.HOLD:
                    writer.WriteValue("Hold");
                    break;
                case GState.HOME:
                    writer.WriteValue("Home");
                    break;
                case GState.ALARM:
                    writer.WriteValue("Alarm");
                    break;
                case GState.CHECK:
                    writer.WriteValue("Check");
                    break;
                case GState.DOOR:
                    writer.WriteValue("Door");
                    break;
                default:
                    writer.WriteValue("Disconnect");
                    break;
            }
        }

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is GState)
            {
                switch (value)
                {
                    case GState.IDLE:
                        return "Idle";
                    case GState.RUN:
                        return "Run";
                    case GState.HOLD:
                        return "Hold";
                    case GState.DOOR:
                        return "Door";
                    case GState.HOME:
                        return "Home";
                    case GState.CHECK:
                        return "Check";
                    case GState.DISCONNECT:
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
                    return GState.IDLE;
                case "run":
                    return GState.RUN;
                case "hold":
                    return GState.HOLD;
                case "door":
                    return GState.DOOR;
                case "home":
                    return GState.HOME;
                case "check":
                    return GState.CHECK;
                case "disconnect":
                    return GState.DISCONNECT;
                default:
                    return GState.ALARM;
            }
        }
    }
}
