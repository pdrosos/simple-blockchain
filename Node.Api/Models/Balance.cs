using System.ComponentModel.DataAnnotations;

namespace Node.Api.Models
{
    public class Balance
    {
        public int Confirmations { get; set; }

        [Display(Name ="Balance")]
        public long BalanceValue { get; set; }
    }
}
