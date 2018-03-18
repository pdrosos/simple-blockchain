namespace Miner.Console
{
    class MiningJob
    {
        public ulong BlockIndex { get; set; }
        
        public int TransactionsIncluded { get; set; }
        
        public int Difficulty { get; set; }

        public ulong ExpectedReward { get; set; }

        public string RewardAddress { get; set; }

        public string BlockDataHash { get; set; }
    }
}
