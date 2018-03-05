using System;

namespace Node.Api.Models
{
    public class MinedBlock
    {
        public string BlockDataHash { get; set; }

        public DateTime DateCreated { get; set; }

        public int Nonce { get; set; }

        public string BlockHash { get; set; }
    }
}
