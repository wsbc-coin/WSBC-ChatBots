using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace WSBC.ChatBots.Coin.Explorer.Services
{
    public class ExplorerDataClient : IExplorerDataClient
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger _log;
        private readonly ExplorerOptions _explorerOptions;

        public ExplorerDataClient(IHttpClientFactory clientFactory, ILogger<ExplorerDataClient> log,
            IOptionsSnapshot<ExplorerOptions> explorerOptions)
        {
            this._clientFactory = clientFactory;
            this._log = log;
            this._explorerOptions = explorerOptions.Value;
        }

        public Task<ExplorerBlockData> GetBlockDataAsync(string blockID, CancellationToken cancellationToken = default)
        {
            this._log.LogDebug("Requesting Block {Block} data from Explorer", blockID);
            return this.SendRequestAsync<ExplorerBlockData>($"block/{blockID}", cancellationToken);
        }

        public Task<ExplorerEmissionData> GetEmissionDataAsync(CancellationToken cancellationToken = default)
        {
            this._log.LogDebug("Requesting Emission data from Explorer");
            return this.SendRequestAsync<ExplorerEmissionData>($"emission", cancellationToken);
        }

        public Task<ExplorerNetworkData> GetNetworkDataAsync(CancellationToken cancellationToken = default)
        {
            this._log.LogDebug("Requesting Network data from Explorer");
            return this.SendRequestAsync<ExplorerNetworkData>("networkinfo", cancellationToken);
        }

        public Task<ExplorerTransactionData> GetTransactionDataAsync(string transactionHash, CancellationToken cancellationToken = default)
        {
            this._log.LogDebug("Requesting Transaction {Transaction} data from Explorer", transactionHash);
            return this.SendRequestAsync<ExplorerTransactionData>($"transaction/{transactionHash}", cancellationToken);
        }

        private async Task<T> SendRequestAsync<T>(string endpoint, CancellationToken cancellationToken = default)
        {
            this._log.LogTrace("Building Explorer request URL");
            Uri url = new Uri($"{this._explorerOptions.ApiURL}/{endpoint}");

            this._log.LogTrace("Sending request to {URL}", url);
            HttpClient client = this._clientFactory.CreateClient();
            using HttpResponseMessage response = await client.GetAsync(url, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            this._log.LogTrace("Parsing Explorer response");
            JObject data = JObject.Parse(await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false));
            if (!data.Value<string>("status").Equals("success", StringComparison.OrdinalIgnoreCase))
            {
                this._log.LogError("Failed receiving data from Explorer");
                return default;
            }
            return data["data"].ToObject<T>();
        }
    }
}
