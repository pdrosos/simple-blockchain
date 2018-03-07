using System;
using System.Collections.Generic;
using Node.Api.Models;
using Node.Api.Services.Abstractions;

namespace Node.Api.Services
{
    public class MockedDataService : IMockedDataService
    {
        private NodeInfo nodeInfo;

        private List<Block> blocks;

        private List<Transaction> pendingTransactions;

        private List<string> peers;

        private MiningJob miningJob;

        private Dictionary<string, Block> miningJobs;

        public MockedDataService()
        {
            this.nodeInfo = new NodeInfo()
            {
                Peers = 2,
                Blocks = 25,
                CumulativeDifficulty = 127,
                ConfirmedTransactions = 208,
                PendingTransactions = 7
            };

            this.blocks = new List<Block>()
            {
                new Block()
                {
                    Index = 17,
                    Transactions = new List<Transaction>()
                    {
                        new Transaction()
                        {
                            From = "44fe0696beb6e24541cc0e8728276c9ec3af2675",
                            To = "9a9f082f37270ff54c5ca4204a0e4da6951fe917",
                            Value = 25000,
                            Fee = 10,
                            DateCreated = new DateTime(2017, 02, 01, 23, 23, 56, 337),
                            TransactionHash = "23fe06345cc864aed086465ff8276c9ec3ac267",
                            TransferSuccessful = true
                        },
                        new Transaction()
                        {
                            From = "32fe0696beb6e24541cc0e8728276c9ec3af2675",
                            To = "4abe00f082f37270ff54c5ca4204a0e4da6951fe23",
                            Value = 42000,
                            Fee = 24,
                            DateCreated = new DateTime(2017, 02, 01, 23, 23, 56, 337),
                            TransactionHash = "56fe0696beb6e24541cc0e8276c9ecaa3345as",
                            TransferSuccessful = true
                        }
                    },
                    Difficulty = 5,
                    PrevBlockHash = "d279fa6a31ae4fb07cfd9cf7f35cc01f…3cf20a",
                    MinedBy = "91c43337992580bca7d5f758d09e88f9b7032a65",
                    BlockDataHash = "5d845cddcd4404ecfd5476fd6b1cf0e5…a80cd3",
                    Nonce = 2455432,
                    DateCreated = new DateTime(2017, 02, 01, 23, 23, 56, 337),
                    BlockHash = "00000abf2f3d86d5c000c0aa7a425a6a4a65…cf4c"
                },
                new Block()
                {
                    Index = 18,
                    Transactions = new List<Transaction>()
                    {
                        new Transaction()
                        {
                            From = "11ce0696beb6e24541cc0e8728276c9ec3af2675",
                            To = "89ac082f37270ff54c5ca4204a0e4da6951fe917",
                            Value = 56000,
                            Fee = 32,
                            DateCreated = new DateTime(2017, 02, 01, 23, 23, 56, 337),
                            TransactionHash = "51acde0696beb66e24541ce8276c9ecaa32aed00",
                            TransferSuccessful = true
                        },
                        new Transaction()
                        {
                            From = "27fe0696beb6e24541cc0e8728276c9ec3af2675",
                            To = "417ba082f37270ff54c5ca4204a0e4da6951fe2ac",
                            Value = 9000,
                            Fee = 11,
                            DateCreated = new DateTime(2017, 02, 01, 23, 23, 56, 337),
                            TransactionHash = "91acc0e872beb66e24541ce8276c9ecaa32aed10",
                            TransferSuccessful = true
                        }
                    },
                    Difficulty = 5,
                    PrevBlockHash = "d279fa6a31ae4fb07cfd9cf7f35cc01f…3cf20a",
                    MinedBy = "91c43337992580bca7d5f758d09e88f9b7032a65",
                    BlockDataHash = "5d845cddcd4404ecfd5476fd6b1cf0e5…a80cd3",
                    Nonce = 2455432,
                    DateCreated = new DateTime(2017, 02, 01, 23, 23, 56, 337),
                    BlockHash = "00000abf2f3d86d5c000c0aa7a425a6a4a65…cf4c"
                }
            };

            this.pendingTransactions = new List<Transaction>()
            {
                new Transaction()
                {
                    From = "66cdfe0696beb6e24541cc0e8728276c9ec3af26",
                    To = "9a9f082f37270ff54c5ca4204a0e4da6951fe917",
                    Value = 16780,
                    Fee = 24,
                    DateCreated = new DateTime(2017, 02, 02, 22, 23, 56, 337),
                    TransactionHash = "49abc06345a4204a0e4da465ff8276c9ec3ac356"
                },
                new Transaction()
                {
                    From = "14fe356beb6e24541cc0e8728276c9ec3af9a00",
                    To = "23af082f37270ff54c5ca4204a0e4da6951fe917",
                    Value = 32400,
                    Fee = 24,
                    DateCreated = new DateTime(2017, 02, 01, 20, 23, 56, 337),
                    TransactionHash = "52ac8ade6e24541cc0e8276c9ecaa3345as"
                },
                new Transaction()
                {
                    From = "174abacedff6beb6e24541cc0e8728276c9ec3aeff2103",
                    To = "1234aedddca37270ff54c5ca4204a0e4da6951ad234",
                    Value = 64500,
                    Fee = 32,
                    DateCreated = new DateTime(2017, 01, 01, 10, 23, 16, 239),
                    TransactionHash = "78bacce94204a066e24541ce8276c9ecaa32aed00a"
                }
            };

            this.peers = new List<string>()
            {
                "http://212.50.11.109:5555",
                "http://af6c7a.ngrok.org:5555"
            };

            this.miningJob = new MiningJob()
            {
                Index = 50,
                TransactionsIncluded = 17,
                ExpectedReward = 5000350,
                RewardAddress = "9a9f08…fe917",
                BlockDataHash = "d2c6ee29ff14b499af985824ea12afccc8…e4cd"
            };

            this.miningJobs = new Dictionary<string, Block>()
            {
                { "293eadecc0045670aced00dea", this.Blocks[0] },
                { "345accde0067aed00ee312312", this.Blocks[1] }
            };
        }

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
