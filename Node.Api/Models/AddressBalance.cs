namespace Node.Api.Models
{
    public class AddressBalance
    {
        public string Address { get; set; }

        public Balance ConfirmedBalance { get; set; }

        public Balance LastMinedBalance { get; set; }

        public Balance PendingBalance { get; set; }
    }
}
