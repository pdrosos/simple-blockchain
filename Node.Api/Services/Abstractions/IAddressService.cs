using Node.Api.Models;

namespace Node.Api.Services.Abstractions
{
    public interface IAddressService
    {
        AddressBalance GetAddressBalance(string address);

        AddressTransactions GetTransactionsForAddress(string address);
    }
}
