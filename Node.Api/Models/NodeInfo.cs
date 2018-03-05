namespace Node.Api.Models
{
    public class NodeInfo
    {
        public string About { get; set; }

        public string NodeUrl { get; set; }

        public int Peers { get; set; }

        public int Difficulty { get; set; }

        public int Blocks { get; set; }

        public int CumulativeDifficulty { get; set; }

        public int ConfirmedTransactions { get; set; }

        public int PendingTransactions { get; set; }
    }
}
