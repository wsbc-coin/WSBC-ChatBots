using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace WSBC.ChatBots.Discord.Utilities
{
    public class UnixTimestampConverter : DateTimeConverterBase
    {
        public static long ToUnixTimestamp(DateTime value)
        {
            double seconds = (value - DateTime.UnixEpoch).TotalSeconds;
            return (long)seconds;
        }

        public static long ToUnixTimestamp(DateTimeOffset value)
            => ToUnixTimestamp(value.DateTime);

        public static DateTime ToDateTime(long value)
            => DateTime.UnixEpoch.AddSeconds(value);

        public static DateTimeOffset ToDateTimeOffset(long value)
            => DateTimeOffset.UnixEpoch.AddSeconds(value);

        /// <inheritdoc/>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else if (value is DateTime dt)
                writer.WriteValue(ToUnixTimestamp(dt));
            else if (value is DateTimeOffset dto)
                writer.WriteValue(ToUnixTimestamp(dto));
            else
                throw new InvalidOperationException();
        }

        /// <inheritdoc/>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
                return null;
            if (objectType == typeof(DateTime) || objectType == typeof(DateTime?))
                return ToDateTime((long)reader.Value);
            if (objectType == typeof(DateTimeOffset) || objectType == typeof(DateTimeOffset?))
                return ToDateTimeOffset((long)reader.Value);
            throw new InvalidOperationException();
        }
    }
}
