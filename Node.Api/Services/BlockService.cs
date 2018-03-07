using Node.Api.Services.Abstractions;

namespace Node.Api.Services
{
    public class BlockService : IBlockService
    {
        private readonly IDataService dataService;

        public BlockService(IDataService dataService)
        {
            this.dataService = dataService;
        }
    }
}
