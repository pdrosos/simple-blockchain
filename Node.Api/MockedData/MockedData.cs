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

        public static readonly List<Transaction> PendingTransactions = new List<Transaction>()
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
    }
}
