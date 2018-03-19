using System.Collections.Generic;

namespace Node.Api.Models
{
    public class NodeInfo
    {
        public string About { get; set; }

        public string NodeUrl { get; set; }

        public int Peers { get; set; }

        public List<string> PeersListUrls { get; set; }

        public int Difficulty { get; set; }

        public int Blocks { get; set; }

        public int CumulativeDifficulty { get; set; }

        public long ConfirmedTransactions { get; set; }

        public int PendingTransactions { get; set; }

        public NodeInfo()
        {
            this.PeersListUrls = new List<string>();
            this.Blocks = 1;
        }
    }
}
