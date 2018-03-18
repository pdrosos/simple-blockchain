using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Node.Api.Models
{
    public class Block
    {
        [JsonProperty(Order = 1)]
        [Required]
        public ulong Index { get; set; }

        [JsonProperty(Order = 2)]
        [Required]
        public List<Transaction> Transactions { get; set; }

        [JsonProperty(Order = 3)]
        [Required]
        public int Difficulty { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, Order = 4)]
        public string PrevBlockHash { get; set; }

        [JsonProperty(Order = 5)]
        public string MinedBy { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, Order = 6)]
        public string BlockDataHash { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, Order = 7)]
        public ulong Nonce { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, Order = 8)]
        public DateTime DateCreated { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, Order = 9)]
        public string BlockHash { get; set; }
    }
}
