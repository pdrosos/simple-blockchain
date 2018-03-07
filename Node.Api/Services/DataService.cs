using System.Collections.Generic;

using Node.Api.Models;
using Node.Api.Services.Abstractions;

namespace Node.Api.Services
{
    public class DataService : IDataService
    {
        private NodeInfo nodeInfo;

        private List<Block> blocks;

        private List<Transaction> pendingTransactions;

        private List<string> peers;

        private MiningJob miningJob;

        private Dictionary<string, Block> miningJobs;

        public NodeInfo NodeInfo
        {
            get { return this.nodeInfo; }
            set { this.nodeInfo = value; }
        }

        public List<Block> Blocks
        {
            get { return this.blocks; }
            set { this.blocks = value; }
        }

        public List<Transaction> PendingTransactions
        {
            get { return this.pendingTransactions; }
            set { this.pendingTransactions = value; }
        }

        public List<string> Peers
        {
            get { return this.peers; }
            set { this.peers = value; }
        }

        public MiningJob MiningJob
        {
            get { return this.miningJob; }
            set { this.miningJob = value; }
        }

        public Dictionary<string, Block> MiningJobs
        {
            get { return this.miningJobs; }
            set { this.miningJobs = value; }
        }
    }
}
