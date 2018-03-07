using Node.Api.Models;
using Node.Api.Services.Abstractions;
using System.Collections.Generic;

namespace Node.Api.Services
{
    public class TransactionService : ITransactionService
    {
        private readonly IDataService dataService;

        public TransactionService(IDataService dataService)
        {
            this.dataService = dataService;
        }

        public string CalculateTransactionHash(Transaction transaction)
        {
            return string.Empty;
        }

        public bool IsCollisionDetected(Transaction transaction)
        {
            return false;
        }

        public void AddTransactionToPendingTransactions(Transaction transaction, List<Transaction> pendingTransactions)
        {
        }

        public void SendTransactionToPeerNodes(List<string> peerNodes)
        {
        }
    }
}
