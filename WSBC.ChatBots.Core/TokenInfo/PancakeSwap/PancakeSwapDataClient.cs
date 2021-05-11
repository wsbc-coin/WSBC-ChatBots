using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WSBC.ChatBots.Token.PancakeSwap
{
    class PancakeSwapDataClient : ITokenDataClient<PancakeSwapData>
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger _log;
        private readonly IOptionsMonitor<PancakeSwapOptions> _pancakeSwapOptions;
        private readonly IOptionsMonitor<TokenOptions> _tokenOptions;

        private readonly JsonSerializer _defaultSerializer;

        public PancakeSwapDataClient(IHttpClientFactory clientFactory, ILogger<PancakeSwapDataClient> log, 
            IOptionsMonitor<PancakeSwapOptions> pancakeSwapOptions, IOptionsMonitor<TokenOptions> tokenOptions)
        {
            this._clientFactory = clientFactory;
            this._log = log;
            this._pancakeSwapOptions = pancakeSwapOptions;
            this._tokenOptions = tokenOptions;

            JsonSerializerSettings serializerSettings = new JsonSerializerSettings();
            serializerSettings.Formatting = Formatting.None;
            this._defaultSerializer = JsonSerializer.CreateDefault(serializerSettings);
        }

        public async Task<PancakeSwapData> GetDataAsync(CancellationToken cancellationToken = default)
        {
            this._log.LogDebug("Requesting token data from PancakeSwap");
            this._log.LogTrace("Building PancakeSwap request URL");
            Uri url = new Uri($"{this._pancakeSwapOptions.CurrentValue.ApiURL}/tokens/{this._tokenOptions.CurrentValue.ContractAddress}");

            this._log.LogTrace("Sending request to {URL}", url);
            HttpClient client = this._clientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("User-Agent", this._pancakeSwapOptions.CurrentValue.UserAgent);

            using HttpResponseMessage response = await client.GetAsync(url, cancellationToken).ConfigureAwait(false);

            this._log.LogTrace("Parsing PancakeSwap response");
            JObject data = JObject.Parse(await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false));
            if (!response.IsSuccessStatusCode)
            {
                JToken errorData = data["error"];
                this._log.LogError("Failed receiving data from PancakeSwap ({Code}): {Message}", (int)response.StatusCode, 
                    errorData?.Value<string>("message") ?? string.Empty);
                return null;
            }

            PancakeSwapData result = data["data"].ToObject<PancakeSwapData>();
            using JsonReader reader = data.CreateReader();
            this._defaultSerializer.Populate(reader, result);
            return result;
        }
    }
}
