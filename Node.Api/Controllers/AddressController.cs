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
            int latestAddressBlockIndex = -1;

            long confirmedBalance = 0L;

            long lastMinedBalance = 0L;

            long pendingBalance = 0L;

            int saveConfirmCount = 6;

            for (int currentBlockIndex = 0; currentBlockIndex < MockedData.Blocks.Count; currentBlockIndex++)
            {
                foreach (var transaction in MockedData.Blocks[currentBlockIndex].Transactions)
                {
                    if (transaction.From == address || transaction.To == address)
                    {
                        latestAddressBlockIndex = currentBlockIndex;

                        if (MockedData.Blocks.Count - currentBlockIndex >= saveConfirmCount)
                        {
                            confirmedBalance = this.UpdateBalance(transaction, address, confirmedBalance);
                        }

                        lastMinedBalance = this.UpdateBalance(transaction, address, lastMinedBalance);

                        pendingBalance = this.UpdateBalance(transaction, address, pendingBalance);
                    }
                }
            }

            for (int currentTransactionIndex = 0; currentTransactionIndex < MockedData.PendingTransactions.Count; currentTransactionIndex++)
            {
                Transaction currentPendingTransaction = MockedData.PendingTransactions[currentTransactionIndex];

                if (currentPendingTransaction.From == address || currentPendingTransaction.To == address)
                {
                    pendingBalance = this.UpdateBalance(currentPendingTransaction, address, pendingBalance);
                }
            }

            int confirmations;

            if (latestAddressBlockIndex >= 0)
            {
                confirmations = MockedData.Blocks.Count - latestAddressBlockIndex;
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

            addressBalance.PendingBalance = new Balance()
            {
                Confirmations = 0,
                BalanceValue = pendingBalance
            };

            return Ok();
        }

        private long UpdateBalance(Transaction transaction, string address, long balance)
        {
            long currentBalance = balance;

            if (transaction.From == address && transaction.To == address)
            {
                return currentBalance;
            }
            else if (transaction.From == address)
            {
                currentBalance -= transaction.Value;
            }
            else if (transaction.To == address)
            {
                currentBalance += transaction.Value;
            }

            return currentBalance;
        }
    }
}
