using System;
using System.Collections.Generic;

namespace Node.Api.Models
{
    public class Block
    {
        public long Index { get; set; }

        public List<Transaction> Transactions { get; set; }

        public int Difficulty { get; set; }

        public string PrevBlockHash { get; set; }

        public string MinedBy { get; set; }

        public string BlockDataHash { get; set; }

        public int Nonce { get; set; }

        public DateTime DateCreated { get; set; }

        public string BlockHash { get; set; }
    }
}
