namespace Node.Api.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;

    using Node.Api.Models;
    using Node.Api.MockedData;

    [Route("api/[controller]")]
    public class BlocksController : Controller
    {
        public IActionResult Get()
        {
            List<Block> blocks = MockedData.Blocks;

            return Ok(blocks);
        }

        [HttpGet("{index}")]
        public IActionResult GetByIndex(long index)
        {
            Block block = MockedData.Blocks.FirstOrDefault(b => b.Index == index);

            return Ok(block);
        }

        [HttpPost("Notify")]
        public IActionResult Notify([FromBody]NewBlockNotification newBlockNotification)
        {
            return Ok(new { Message = "Thank you for the notification." });
        }
    }
}
