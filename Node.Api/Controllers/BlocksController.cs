namespace Node.Api.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;

    using Node.Api.Models;
    using Node.Api.Services.Abstractions;

    [Route("[controller]")]
    public class BlocksController : Controller
    {
        private readonly IDataService dataService;

        private readonly INodeService nodeService;

        public BlocksController(IDataService dataService, INodeService nodeService)
        {
            this.dataService = dataService;
            this.nodeService = nodeService;
        }

        // GET blocks
        [HttpGet]
        public IActionResult Get()
        {
            List<Block> blocks = this.dataService.Blocks;

            return Ok(blocks);
        }

        // GET blocks/17
        [HttpGet("{index}")]
        public IActionResult GetByIndex(ulong index)
        {
            Block block = this.dataService.Blocks.FirstOrDefault(b => b.Index == index);

            if (block == null)
            {
                return NotFound();
            }

            return Ok(block);
        }

        // POST notify
        [HttpPost("notify")]
        public IActionResult Notify([FromBody]NewBlockNotification newBlockNotification)
        {
            this.nodeService.ReceiveNewBlock(newBlockNotification);
            
            return Ok();
        }
    }
}
