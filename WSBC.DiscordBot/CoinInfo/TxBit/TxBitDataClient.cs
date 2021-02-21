using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace WSBC.DiscordBot.TxBit.Services
{
    class TxBitDataClient : ICoinDataClient<TxBitData>
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger _log;
        private readonly WsbcOptions _wsbcOptions;
        private readonly TxBitOptions _txbitOptions;

        public TxBitDataClient(IHttpClientFactory clientFactory, ILogger<TxBitDataClient> log,
            IOptionsSnapshot<WsbcOptions> wsbcOptions, IOptionsSnapshot<TxBitOptions> txbitOptions)
        {
            this._clientFactory = clientFactory;
            this._log = log;
            this._wsbcOptions = wsbcOptions.Value;
            this._txbitOptions = txbitOptions.Value;
        }

        public async Task<TxBitData> GetDataAsync(CancellationToken cancellationToken = default)
        {
            this._log.LogDebug("Requesting coin data from TxBit");
            this._log.LogTrace("Building TxBit request URL");
            Uri url = new Uri($"{this._txbitOptions.ApiURL}/getcurrencyinformation?currency={this._wsbcOptions.CoinCode}");

            this._log.LogTrace("Sending request to {URL}", url);
            HttpClient client = this._clientFactory.CreateClient();
            using HttpResponseMessage response = await client.GetAsync(url, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            this._log.LogTrace("Parsing TxBit response");
            JObject data = JObject.Parse(await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false));
            if (!data.Value<bool>("success"))
            {
                this._log.LogError("Failed receiving data from TxBit: {Message}", data.Value<string>("message"));
                return null;
            }
            return data["result"].ToObject<TxBitData>();
        }
    }
}
