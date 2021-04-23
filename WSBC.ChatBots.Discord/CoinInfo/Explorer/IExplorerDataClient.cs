using System.Threading;
using System.Threading.Tasks;

namespace WSBC.ChatBots.Coin.Explorer
{
    public interface IExplorerDataClient
    {
        Task<ExplorerNetworkData> GetNetworkDataAsync(CancellationToken cancellationToken = default);
        Task<ExplorerBlockData> GetBlockDataAsync(string blockID, CancellationToken cancellationToken = default);
        Task<ExplorerTransactionData> GetTransactionDataAsync(string transactionHash, CancellationToken cancellationToken = default);
        Task<ExplorerEmissionData> GetEmissionDataAsync(CancellationToken cancellationToken = default);
    }
}
