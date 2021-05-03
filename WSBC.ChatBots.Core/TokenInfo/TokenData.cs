namespace WSBC.ChatBots.Token
{
    public class TokenData
    {
        /// <summary>Tokens traded in last 24 hours.</summary>
        public double Volume { get; init; }
        /// <summary>Price of token in USD.</summary>
        public decimal Price { get; init; }
        /// <summary>Percentage price change.</summary>
        public double Change { get; init; }
    }
}
