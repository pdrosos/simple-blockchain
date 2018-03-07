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

        public Transaction SignTransaction(Transaction transaction)
        {
            List<string> senderSignature = this.CalculateSenderSignature(transaction);

            var signedTransaction = transaction;

            signedTransaction.SenderSignature = senderSignature;

            return signedTransaction;
        }

        private List<string> CalculateSenderSignature(Transaction transaction)
        {
            return new List<string>();
        }
    }
}
