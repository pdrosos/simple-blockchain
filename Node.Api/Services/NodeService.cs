using Node.Api.Services.Abstractions;

namespace Node.Api.Services
{
    public class NodeService : INodeService
    {
        private readonly IDataService dataService;

        public NodeService(IDataService dataService)
        {
            this.dataService = dataService;
        }
    }
}
