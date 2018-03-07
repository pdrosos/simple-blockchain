using Node.Api.Services.Abstractions;

namespace Node.Api.Services
{
    public class MiningService : IMiningService
    {
        private readonly IDataService dataService;

        public MiningService(IDataService dataService)
        {
            this.dataService = dataService;
        }
    }
}
