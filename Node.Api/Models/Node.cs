using System.Collections.Generic;

namespace Node.Api.Models
{
    public class Node
    {
        public List<string> Peers { get; set; }

        public List<Block> Blocks { get; set; }

        public List<Transaction> PendingTransactions { get; set; }

        public int Difficulty { get; set; }

        public Dictionary<string, Block> MiningJobs { get; set; }
    }
}
