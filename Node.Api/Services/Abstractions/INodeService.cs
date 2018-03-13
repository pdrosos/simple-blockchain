using Node.Api.Models;

namespace Node.Api.Services.Abstractions
{
    public interface INodeService
    {
        TransactionSubmissionResponse ProcessTransaction(Transaction transaction, string currentPeerUrl);
    }
}
