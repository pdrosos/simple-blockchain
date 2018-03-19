using System.Threading.Tasks;
using Node.Api.Models;

namespace Node.Api.Services.Abstractions
{
    public interface INodeService
    {
        TransactionSubmissionResponse ProcessTransaction(Transaction transaction, string currentPeerUrl);

        MiningJob GetMiningJob(string minerAddress);

        void VerifyMinedJob(MinedBlockPostModel minedBlock);

        void ReceiveNewBlock(NewBlockNotification newBlockNotification);

        void GenerateGenesisBlock();
    }
}
