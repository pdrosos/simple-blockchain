namespace Node.Api.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;

    using Node.Api.Models;
    using Node.Api.Services.Abstractions;

    [Route("api/[controller]")]
    public class BlocksController : Controller
    {
        private readonly IMockedDataService mockedDataService;

        public BlocksController(IMockedDataService mockedDataService)
        {
            this.mockedDataService = mockedDataService;
        }

        // GET api/blocks
        [HttpGet]
        public IActionResult Get()
        {
            List<Block> blocks = this.mockedDataService.Blocks;

            return Ok(blocks);
        }

        // GET api/blocks/17
        [HttpGet("{index}")]
        public IActionResult GetByIndex(long index)
        {
            Block block = this.mockedDataService.Blocks.FirstOrDefault(b => b.Index == index);

            if (block == null)
            {
                return NotFound();
            }

            return Ok(block);
        }

        // POST api/notify
        [HttpPost("notify")]
        public IActionResult Notify([FromBody]NewBlockNotification newBlockNotification)
        {
            return Ok(new { Message = "Thank you for the notification." });
        }
    }
}
