using Node.Api.Models;
using System;
using System.Collections.Generic;

namespace Node.Api.MockedData
{
    public static class MockedData
    {
        public static readonly NodeInfo NodeInfo = new NodeInfo()
        {
            Peers = 2,
            Blocks = 25,
            CumulativeDifficulty = 127,
            ConfirmedTransactions = 208,
            PendingTransactions = 7
        };

        public static readonly List<Block> Blocks = new List<Block>()
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
                        DateCreated = new DateTime(2017, 02, 01, 23, 23, 56, 337)
                    },
                    new Transaction()
                    {
                        From = "44fe0696beb6e24541cc0e8728276c9ec3af2675",
                        To = "9a9f082f37270ff54c5ca4204a0e4da6951fe917",
                        Value = 42000,
                        Fee = 24,
                        DateCreated = new DateTime(2017, 02, 01, 23, 23, 56, 337),
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
                        From = "44fe0696beb6e24541cc0e8728276c9ec3af2675",
                        To = "9a9f082f37270ff54c5ca4204a0e4da6951fe917",
                        Value = 56000,
                        Fee = 32,
                        DateCreated = new DateTime(2017, 02, 01, 23, 23, 56, 337),
                    },
                    new Transaction()
                    {
                        From = "44fe0696beb6e24541cc0e8728276c9ec3af2675",
                        To = "9a9f082f37270ff54c5ca4204a0e4da6951fe917",
                        Value = 9000,
                        Fee = 11,
                        DateCreated = new DateTime(2017, 02, 01, 23, 23, 56, 337),
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
    }
}
