namespace Node.Api.Controllers
{
    using System.Collections.Generic;

    using Microsoft.AspNetCore.Mvc;
    using Node.Api.MockedData;
    using Node.Api.Models;
    using Microsoft.AspNetCore.Http;

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
            var connectedPeers = MockedData.Peers;

            if (connectedPeers.Contains(peer.PeerUrl))
            {
                return StatusCode(StatusCodes.Status409Conflict);
            }

            return Ok(new { Message = string.Format("Added peer: {0}", peer.PeerUrl) });
        }
    }
}
