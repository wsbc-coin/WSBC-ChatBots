using System;
using Newtonsoft.Json;

namespace WSBC.ChatBots.Coin.Serialization
{
    class TransactionAmountConverter : JsonConverter
    {
        private const int _offset = 1000000000;

        public override bool CanConvert(Type objectType)
            => IsDecimal(objectType) || IsDouble(objectType) || IsSingle(objectType);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.Value == null)
                return null;
            if (IsDecimal(objectType))
                return Convert.ToDecimal(reader.Value) / _offset;
            if (IsDouble(objectType))
                return Convert.ToDouble(reader.Value) / _offset;
            if (IsSingle(objectType))
                return Convert.ToSingle(reader.Value) / _offset;
            throw new InvalidCastException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            long rawValue = (long)((decimal)value * _offset);
            writer.WriteValue(rawValue);
        }

        private static bool IsDecimal(Type objectType)
            => objectType == typeof(decimal) || objectType == typeof(decimal?);
        private static bool IsDouble(Type objectType)
            => objectType == typeof(double) || objectType == typeof(double?);
        private static bool IsSingle(Type objectType)
            => objectType == typeof(float) || objectType == typeof(float?);
    }
}
