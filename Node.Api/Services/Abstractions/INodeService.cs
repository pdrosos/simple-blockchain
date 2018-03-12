using Node.Api.Models;

namespace Node.Api.Services.Abstractions
{
    public interface INodeService
    {
        TransactionSubmissionResponse AddTransaction(Transaction transaction);

        void SendTransactionToPeers(Transaction transaction, string currentPeerUrl);
    }
}
