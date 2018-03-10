using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Node.Api.Models
{
    public class Balance
    {
        public int Confirmations { get; set; }

        [Display(Name ="Balance")]
        [JsonProperty(PropertyName = "balance")]
        public long BalanceValue { get; set; }
    }
}
