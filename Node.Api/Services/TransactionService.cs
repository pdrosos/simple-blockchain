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
    }
}
