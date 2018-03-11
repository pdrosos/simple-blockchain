using Newtonsoft.Json;

namespace Node.Api.Models
{
    public class TransactionSignatureDataModel
    {
        [JsonProperty(Order = 1)]
        public string From { get; set; }

        [JsonProperty(Order = 2)]
        public string To { get; set; }

        [JsonProperty(Order = 3)]
        public string SenderPubKey { get; set; }

        [JsonProperty(Order = 4)]
        public long Value { get; set; }

        [JsonProperty(Order = 5)]
        public long Fee { get; set; }

        [JsonProperty(Order = 6)]
        public string DateCreated { get; set; }
    }
}
