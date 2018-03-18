namespace Miner.Console.Models
{
    public class MinedBlockPostModel
    {
        public string BlockDataHash { get; set; }

        public string DateCreated { get; set; }

        public ulong Nonce { get; set; }
    }
}
