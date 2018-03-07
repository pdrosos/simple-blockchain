namespace Node.Api.Controllers
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;

    using Node.Api.Models;
    using Microsoft.AspNetCore.Http;
    using Node.Api.Services.Abstractions;

    [Route("[controller]")]
    public class PeersController : Controller
    {
        private readonly IMockedDataService mockedDataService;

        private readonly IPeerService peerService;

        public PeersController(IMockedDataService mockedDataService, IPeerService peerService)
        {
            this.mockedDataService = mockedDataService;

            this.peerService = peerService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            List<string> peers = this.mockedDataService.Peers;

            return Ok(peers);
        }

        [HttpPost]
        public IActionResult ConnectPeer([FromBody]Peer peer)
        {
            var connectedPeers = this.mockedDataService.Peers;

            if (connectedPeers.Contains(peer.PeerUrl))
            {
                return StatusCode(StatusCodes.Status409Conflict);
            }

            return Ok(new { Message = string.Format("Added peer: {0}", peer.PeerUrl) });
        }
    }
}
