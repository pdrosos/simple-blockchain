using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Node.Api.Models
{
    public class Transaction
    {
        public string From { get; set; }

        public string To { get; set; }

        public long Value { get; set; }

        public long Fee { get; set; }

        public DateTime DateCreated { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string SenderPubKey { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<string> SenderSignature { get; set; }

        public string TransactionHash { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public long? MinedInBlockIndex { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? TransferSuccessful { get; set; }
    }
}
