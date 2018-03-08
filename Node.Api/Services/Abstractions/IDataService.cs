using System.Collections.Generic;
using Node.Api.Models;

namespace Node.Api.Services.Abstractions
{
    public interface IDataService
    {
        NodeInfo NodeInfo { get; set; }

        List<Block> Blocks { get; set; }

        List<Transaction> PendingTransactions { get; set; }

        MiningJob MiningJob { get; set; }

        Dictionary<string, Block> MiningJobs { get; set; }
    }
}
