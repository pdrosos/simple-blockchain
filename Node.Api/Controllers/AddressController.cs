namespace Node.Api.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Node.Api.Models;
    using Node.Api.Services.Abstractions;

    [Route("api/[controller]")]
    public class AddressController : Controller
    {
        private readonly IAddressService addressService;

        public AddressController(IAddressService addressService)
        {
            this.addressService = addressService;
        }

        // GET api/address/9a9f082f37270ff54c5ca4204a0e4da6951fe917/transactions
        [HttpGet("{address}/transactions")]
        public IActionResult GetTransactionsForAddress(string address)
        {
            AddressTransactions addressTransactions = this.addressService.GetTransactionsForAddress(address);

            return Ok(addressTransactions);
        }

        // GET api/9a9f082f37270ff54c5ca4204a0e4da6951fe917/balance
        [HttpGet("{address}/balance")]
        public IActionResult GetAddressBalance(string address)
        {
            AddressBalance addressBalance = this.addressService.GetAddressBalance(address);

            return Ok(addressBalance);
        }
    }
}
