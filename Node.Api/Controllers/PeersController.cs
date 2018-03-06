namespace Node.Api.Controllers
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Mvc;
    using Node.Api.MockedData;
    using Node.Api.Models;

    [Route("api/[controller]")]
    public class PeersController : Controller
    {
        [HttpGet]
        public IActionResult Get()
        {
            List<string> peers = MockedData.Peers;

            return Ok(peers);
        }

        [HttpPost]
        public IActionResult ConnectPeer([FromBody]Peer peer)
        {
            return Ok();
        }
    }
}
