using Node.Api.Services.Abstractions;

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
