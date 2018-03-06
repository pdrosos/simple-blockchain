namespace Node.Api.Models
{
    public class MiningJob
    {
        public int Index { get; set; }

        public int TransactionsIncluded { get; set; }

        public long ExpectedReward { get; set; }

        public string RewardAddress { get; set; }

        public string BlockDataHash { get; set; }
    }
}
