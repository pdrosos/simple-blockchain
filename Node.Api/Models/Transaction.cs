using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Node.Api.Models
{
    public class Transaction
    {
        [JsonProperty(Order = 1)]
        [Required(ErrorMessage = "Invalid transaction: field 'from' is missing")]
        public string From { get; set; }

        [JsonProperty(Order = 2)]
        [Required(ErrorMessage = "Invalid transaction: field 'to' is missing")]
        public string To { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, Order = 3)]
        public string SenderPubKey { get; set; }

        [JsonProperty(Order = 4)]
        [Required(ErrorMessage = "Invalid transaction: field 'value' is missing")]
        public long Value { get; set; }

        [JsonProperty(Order = 5)]
        [Required(ErrorMessage = "Invalid transaction: field 'fee' is missing")]
        public long Fee { get; set; }

        [JsonProperty(Order = 6)]
        [Required(ErrorMessage = "Invalid transaction: field 'dateCreated' is missing")]
        public DateTime DateCreated { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, Order = 7)]
        public string[] SenderSignature { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, Order = 8)]
        public string TransactionHash { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, Order = 9)]
        public long? MinedInBlockIndex { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, Order = 10)]
        public bool? TransferSuccessful { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, Order = 11)]
        public List<string> AlreadySentToPeers { get; set; }
    }
}
