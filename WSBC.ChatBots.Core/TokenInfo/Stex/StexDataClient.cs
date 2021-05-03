using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace WSBC.ChatBots.Token.Stex.Services
{
    class StexDataClient : ITokenDataClient<StexData>
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger _log;
        private readonly IOptionsMonitor<StexOptions> _stexOptions;

        public StexDataClient(IHttpClientFactory clientFactory, ILogger<StexDataClient> log, IOptionsMonitor<StexOptions> stexOptions)
        {
            this._clientFactory = clientFactory;
            this._log = log;
            this._stexOptions = stexOptions;
        }

        public async Task<StexData> GetDataAsync(CancellationToken cancellationToken = default)
        {
            this._log.LogDebug("Requesting token data from STEX");
            this._log.LogTrace("Building STEX request URL");
            Uri url = new Uri($"{this._stexOptions.CurrentValue.ApiURL}/ticker/{this._stexOptions.CurrentValue.PairID}");

            this._log.LogTrace("Sending request to {URL}", url);
            HttpClient client = this._clientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("User-Agent", this._stexOptions.CurrentValue.UserAgent);

            using HttpResponseMessage response = await client.GetAsync(url, cancellationToken).ConfigureAwait(false);

            this._log.LogTrace("Parsing STEX response");
            JObject data = JObject.Parse(await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false));
            if (!response.IsSuccessStatusCode)
            {
                this._log.LogError("Failed receiving data from STEX ({Code}): {Message}", (int)response.StatusCode, data.Value<string>("message") ?? string.Empty);
                return null;
            }
            if (!data.Value<bool>("success"))
            {
                this._log.LogError("Failed receiving data from STEX: {Message}", data.Value<string>("message"));
                return null;
            }

            return data["data"].ToObject<StexData>();
        }
    }
}
