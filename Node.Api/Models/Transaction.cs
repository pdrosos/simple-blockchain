using System;
using System.Collections.Generic;

namespace Node.Api.Models
{
    public class Transaction
    {
        public string From { get; set; }

        public string To { get; set; }

        public int Value { get; set; }

        public int Fee { get; set; }

        public DateTime DateCreated { get; set; }

        public string SenderPubKey { get; set; }

        public List<string> SenderSignature { get; set; }

        public string TransactionHash { get; set; }

        public int? MinedInBlockIndex { get; set; }

        public bool? TransferSuccessful { get; set; }
    }
}
