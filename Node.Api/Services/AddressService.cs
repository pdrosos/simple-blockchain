namespace Node.Api.Services
{
    using System.Collections.Generic;
    using System.Linq;

    using Node.Api.Models;
    using Node.Api.Services.Abstractions;

    public class AddressService : IAddressService
    {
        private readonly IMockedDataService mockedDataService;

        private readonly IDataService dataService;

        public AddressService(IMockedDataService mockedDataService, IDataService dataService)
        {
            this.mockedDataService = mockedDataService;

            this.dataService = dataService;
        }

        public AddressTransactions GetTransactionsForAddress(string address)
        {
            List<Transaction> transactionsForAddressFromBlocks = this.mockedDataService.Blocks
                .SelectMany(b => b.Transactions)
                .Where(t => t.From == address || t.To == address)
                .ToList();

            List<Transaction> transactionsForAddressFromPendingTransactions = this.mockedDataService.PendingTransactions
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

            return addressTransactions;
        }

        public AddressBalance GetAddressBalance(string address)
        {
            int latestAddressBlockIndex = -1;

            long confirmedBalance = 0L;

            long lastMinedBalance = 0L;

            long pendingBalance = 0L;

            int saveConfirmCount = 6;

            for (int currentBlockIndex = 0; currentBlockIndex < this.mockedDataService.Blocks.Count; currentBlockIndex++)
            {
                foreach (var transaction in this.mockedDataService.Blocks[currentBlockIndex].Transactions)
                {
                    if (transaction.From == address || transaction.To == address)
                    {
                        latestAddressBlockIndex = currentBlockIndex;

                        if (this.mockedDataService.Blocks.Count - currentBlockIndex >= saveConfirmCount)
                        {
                            confirmedBalance = this.UpdateBalance(transaction, address, confirmedBalance);
                        }

                        lastMinedBalance = this.UpdateBalance(transaction, address, lastMinedBalance);

                        pendingBalance = this.UpdateBalance(transaction, address, pendingBalance);
                    }
                }
            }

            for (int currentTransactionIndex = 0; currentTransactionIndex < this.mockedDataService.PendingTransactions.Count; currentTransactionIndex++)
            {
                Transaction currentPendingTransaction = this.mockedDataService.PendingTransactions[currentTransactionIndex];

                if (currentPendingTransaction.From == address || currentPendingTransaction.To == address)
                {
                    pendingBalance = this.UpdateBalance(currentPendingTransaction, address, pendingBalance);
                }
            }

            int confirmations;

            if (latestAddressBlockIndex >= 0)
            {
                confirmations = this.mockedDataService.Blocks.Count - latestAddressBlockIndex;
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

            return addressBalance;
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
