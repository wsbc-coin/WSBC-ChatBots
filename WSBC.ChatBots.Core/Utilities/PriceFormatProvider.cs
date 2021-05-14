using System;
using System.Globalization;

namespace WSBC.ChatBots.Utilities
{
    public class PriceFormatProvider : IFormatProvider
    {
        public static PriceFormatProvider Default { get; } = new PriceFormatProvider();

        public string CardinalPrice { get; } = "#,0";
        public string ShortPrice { get; } = "#,0.##";
        public string NormalPrice { get; } = "#,0.00##";
        public string LongPrice { get; } = "#,0.00####";

        private readonly NumberFormatInfo _internalProvider;

        public PriceFormatProvider()
        {
            this._internalProvider = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            this._internalProvider.NumberGroupSeparator = " ";
            this._internalProvider.CurrencyGroupSeparator = " ";
            this._internalProvider.PercentGroupSeparator = " ";
        }

        public object GetFormat(Type formatType)
            => this._internalProvider.GetFormat(formatType);

        public string FormatCardinal(decimal price)
            => price.ToString(this.CardinalPrice, this);
        public string FormatShort(decimal price)
            => price.ToString(this.ShortPrice, this);
        public string FormatNormal(decimal price)
            => price.ToString(this.NormalPrice, this);
        public string FormatLong(decimal price)
            => price.ToString(this.LongPrice, this);
        public string FormatVeryLong(decimal price)
            => price.ToString(this);

        public string FormatCardinal(double price)
            => price.ToString(this.CardinalPrice, this);
        public string FormatShort(double price)
            => price.ToString(this.ShortPrice, this);
        public string FormatNormal(double price)
            => price.ToString(this.NormalPrice, this);
        public string FormatLong(double price)
            => price.ToString(this.LongPrice, this);
        public string FormatVeryLong(double price)
            => price.ToString(this);
    }
}
