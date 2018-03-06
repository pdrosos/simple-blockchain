using System.Collections.Generic;

namespace Node.Api.Models
{
    public class AddressTransactions
    {
        public string Address { get; set; }

        public List<Transaction> Transactions { get; set; }
    }
}
