using Node.Api.Services.Abstractions;

namespace Node.Api.Services
{
    public class PeerService : IPeerService
    {
        private readonly IDataService dataService;

        public PeerService(IDataService dataService)
        {
            this.dataService = dataService;
        }
    }
}