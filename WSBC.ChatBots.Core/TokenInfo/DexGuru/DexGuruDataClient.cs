using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace WSBC.ChatBots.Token.DexGuru.Services
{
    class DexGuruDataClient : ITokenDataClient<DexGuruData>
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger _log;
        private readonly TokenOptions _tokenOptions;
        private readonly DexGuruOptions _dexGuruOptions;

        public DexGuruDataClient(IHttpClientFactory clientFactory, ILogger<DexGuruDataClient> log,
            IOptionsSnapshot<TokenOptions> tokenOptions, IOptionsSnapshot<DexGuruOptions> dexGuruOptions)
        {
            this._clientFactory = clientFactory;
            this._log = log;
            this._tokenOptions = tokenOptions.Value;
            this._dexGuruOptions = dexGuruOptions.Value;
        }

        public async Task<DexGuruData> GetDataAsync(CancellationToken cancellationToken = default)
        {
            this._log.LogDebug("Requesting coin data from dex.guru");
            this._log.LogTrace("Building dex.guru request URL");
            Uri url = new Uri($"{this._dexGuruOptions.ApiURL}/v1/tokens/{this._tokenOptions.ContractAddress}");

            this._log.LogTrace("Sending request to {URL}", url);
            HttpClient client = this._clientFactory.CreateClient();
            using HttpResponseMessage response = await client.GetAsync(url, cancellationToken).ConfigureAwait(false);

            this._log.LogTrace("Parsing dex.guru response");
            JObject data = JObject.Parse(await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false));
            if (!response.IsSuccessStatusCode)
            {
                this._log.LogError("Failed receiving data from dex.guru ({Code}): {Message}", (int)response.StatusCode, data.Value<string>("message"));
                return null;
            }

            return data.ToObject<DexGuruData>();
        }
    }
}
