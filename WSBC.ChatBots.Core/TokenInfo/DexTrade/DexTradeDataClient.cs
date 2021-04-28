using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace WSBC.ChatBots.Token.DexTrade.Services
{
    class DexTradeDataClient : ITokenDataClient<DexTradeData>
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger _log;
        private readonly IOptionsMonitor<DexTradeOptions> _dexTradeOptions;

        public DexTradeDataClient(IHttpClientFactory clientFactory, ILogger<DexTradeDataClient> log, IOptionsMonitor<DexTradeOptions> dexTradeOptions)
        {
            this._clientFactory = clientFactory;
            this._log = log;
            this._dexTradeOptions = dexTradeOptions;
        }

        public async Task<DexTradeData> GetDataAsync(CancellationToken cancellationToken = default)
        {
            this._log.LogDebug("Requesting token data from Dex-Trade");
            this._log.LogTrace("Building Dex-Trade request URL");
            Uri url = new Uri($"{this._dexTradeOptions.CurrentValue.ApiURL}/ticker?pair=WSBTUSDT");

            this._log.LogTrace("Sending request to {URL}", url);
            HttpClient client = this._clientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("User-Agent", this._dexTradeOptions.CurrentValue.UserAgent);

            using HttpResponseMessage response = await client.GetAsync(url, cancellationToken).ConfigureAwait(false);

            this._log.LogTrace("Parsing dex.guru response");
            JObject data = JObject.Parse(await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false));
            if (!response.IsSuccessStatusCode)
            {
                this._log.LogError("Failed receiving data from Dex-Trade ({Code}): {Message}", (int)response.StatusCode, data.Value<string>("error") ?? string.Empty);
                return null;
            }
            if (!data.Value<bool>("status"))
            {
                this._log.LogError("Failed receiving data from Dex-Trade: {Message}", data.Value<string>("error"));
                return null;
            }

            return data["data"].ToObject<DexTradeData>();
        }
    }
}
