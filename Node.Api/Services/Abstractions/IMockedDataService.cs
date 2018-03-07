using Node.Api.Models;
using System.Collections.Generic;

namespace Node.Api.Services.Abstractions
{
    public interface IMockedDataService
    {
        NodeInfo NodeInfo { get; set; }

        List<Block> Blocks { get; set; }

        List<Transaction> PendingTransactions { get; set; }

        List<string> Peers { get; set; }

        MiningJob MiningJob { get; set; }

        Dictionary<string, Block> MiningJobs { get; set; }
    }
}
