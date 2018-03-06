namespace Node.Api.Controllers
{
    using System.Linq;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;

    using Node.Api.Models;
    using Node.Api.MockedData;

    [Route("api/[controller]")]
    public class AddressController : Controller
    {
        // GET api/address/9a9f082f37270ff54c5ca4204a0e4da6951fe917/transactions
        [HttpGet("{address}/transactions")]
        public IActionResult GetTransactionsForAddress(string address)
        {
            List<Transaction> transactionsForAddressFromBlocks = MockedData.Blocks
                .SelectMany(b => b.Transactions)
                .Where(t => t.From == address || t.To == address)
                .ToList();

            List<Transaction> transactionsForAddressFromPendingTransactions = MockedData.PendingTransactions
                .Where(t => t.From == address || t.To == address)
                .ToList();

            List<Transaction> transactionsForAddress = transactionsForAddressFromBlocks
                .Concat(transactionsForAddressFromPendingTransactions)
                .OrderByDescending(t => t.DateCreated)
                .ToList();

            var addressTransactions = new AddressTransactions()
            {
                Address = address,
                Transactions = transactionsForAddress
            };

            return Ok(addressTransactions);
        }

        [HttpGet("{address}/balance")]
        public IActionResult GetAddressBalance(string address)
        {
            int latestTransactionOccurrenceIndex = -1;

            long confirmedBalance = 0;

            long lastMinedBalance = 0;

            int saveConfirmCount = 6;

            for (int i = 0; i < MockedData.Blocks.Count; i++)
            {
                var transaction = MockedData.Blocks[i].Transactions
                    .FirstOrDefault(t => t.From == address || t.To == address);

                if (transaction != null)
                {
                    latestTransactionOccurrenceIndex = i;

                    if (MockedData.Blocks.Count - i >= saveConfirmCount)
                    {
                        confirmedBalance = confirmedBalance + transaction.Value;
                    }

                    lastMinedBalance = lastMinedBalance + transaction.Value;
                }
            }

            int confirmations;

            if (latestTransactionOccurrenceIndex >= 0)
            {
                confirmations = MockedData.Blocks.Count - latestTransactionOccurrenceIndex;
            }
            else
            {
                confirmations = 0;
            }

            var addressBalance = new AddressBalance();

            addressBalance.ConfirmedBalance = new Balance()
            {
                Confirmations = confirmations,
                BalanceValue = confirmedBalance
            };

            addressBalance.LastMinedBalance = new Balance()
            {
                Confirmations = confirmations,
                BalanceValue = lastMinedBalance
            };

            return Ok();
        }
    }
}
