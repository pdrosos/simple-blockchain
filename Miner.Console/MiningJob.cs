namespace Miner.Console
{
    class MiningJob
    {
        public int BlockIndex { get; set; }
        public int TransactionsIncluded { get; set; }
        public ulong ExpectedReward { get; set; }
        public string RewardAddress { get; set; }
        public string BlockDataHash { get; set; }
        public int Difficulty { get; set; }
    }
}
