using System;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace WSBC.ChatBots.Coin.MiningPoolStats.Services
{
    internal class MiningPoolStatsDataClient : ICoinDataClient<MiningPoolStatsData>
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger _log;
        private readonly MiningPoolStatsOptions _options;

        private static readonly Regex _timestampRegex = new Regex(@"var last_time\s?=\s?""(\d+)"";", RegexOptions.CultureInvariant);

        public MiningPoolStatsDataClient(IHttpClientFactory clientFactory, ILogger<MiningPoolStatsDataClient> log,
            IOptionsSnapshot<MiningPoolStatsOptions> options)
        {
            this._clientFactory = clientFactory;
            this._log = log;
            this._options = options.Value;
        }

        public async Task<MiningPoolStatsData> GetDataAsync(CancellationToken cancellationToken = default)
        {
            this._log.LogDebug("Requesting pools data from MiningPoolStats");
            HttpClient client = this._clientFactory.CreateClient();

            // need timestamp, otherwise data endpoint will return forbidden
            long timestamp = await GetTimestampAsync(client, cancellationToken).ConfigureAwait(false);

            this._log.LogTrace("Building MiningPoolStats request URL");
            Uri url = new Uri($"{this._options.ApiURL}?t={timestamp}");

            this._log.LogTrace("Sending request to {URL}", url);
            using HttpResponseMessage response = await client.GetAsync(url, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            this._log.LogTrace("Parsing MiningPoolStats response");
            JObject data = JObject.Parse(await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false));
            return data.ToObject<MiningPoolStatsData>();
        }

        private async Task<long> GetTimestampAsync(HttpClient client, CancellationToken cancellationToken = default)
        {
            string url = this._options.TimestampURL;
            this._log.LogTrace("Getting latest timestamp from {URL}", url);
            using HttpResponseMessage response = await client.GetAsync(url, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            this._log.LogTrace("Parsing MiningPoolStats timestamp");
            string html = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            Match match = _timestampRegex.Match(html);
            string timestamp = match.Groups[1].Value;
            return long.Parse(timestamp);
        }
    }
}
