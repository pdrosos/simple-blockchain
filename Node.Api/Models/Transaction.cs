using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

namespace Node.Api.Models
{
    public class Transaction
    {
        [Required(ErrorMessage = "Invalid transaction: field 'from' is missing")]
        [JsonProperty(Order = 1)]
        public string From { get; set; }

        [Required(ErrorMessage = "Invalid transaction: field 'to' is missing")]
        [JsonProperty(Order = 2)]
        public string To { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, Order = 3)]
        public string SenderPubKey { get; set; }

        [Required(ErrorMessage = "Invalid transaction: field 'value' is missing")]
        [JsonProperty(Order = 4)]
        public long Value { get; set; }

        [Required(ErrorMessage = "Invalid transaction: field 'fee' is missing")]
        [JsonProperty(Order = 5)]
        public long Fee { get; set; }

        [Required(ErrorMessage = "Invalid transaction: field 'dateCreated' is missing")]
        [JsonProperty(Order = 6)]
        public DateTime DateCreated { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, Order = 7)]
        public List<string> SenderSignature { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, Order = 8)]
        public string TransactionHash { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, Order = 9)]
        public long? MinedInBlockIndex { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore, Order = 10)]
        public bool? TransferSuccessful { get; set; }
    }
}
