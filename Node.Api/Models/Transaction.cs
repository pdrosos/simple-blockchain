using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace Node.Api.Models
{
    public class Transaction
    {
        [Required(ErrorMessage = "Invalid transaction: field 'from' is missing")]
        public string From { get; set; }

        [Required(ErrorMessage = "Invalid transaction: field 'to' is missing")]
        public string To { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SenderPubKey { get; set; }

        [Required(ErrorMessage = "Invalid transaction: field 'value' is missing")]
        public long Value { get; set; }

        [Required(ErrorMessage = "Invalid transaction: field 'fee' is missing")]
        public long Fee { get; set; }

        [Required(ErrorMessage = "Invalid transaction: field 'dateCreated' is missing")]
        public DateTime DateCreated { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string[] SenderSignature { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string TransactionHash { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long? MinedInBlockIndex { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? TransferSuccessful { get; set; }
    }
}
