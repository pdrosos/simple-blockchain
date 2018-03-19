namespace Node.Api.Controllers
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;

    using Infrastructure.Library.Helpers;
    using Node.Api.Helpers;
    using Node.Api.Models;
    using Node.Api.Services.Abstractions;

    [Route("[controller]")]
    public class PeersController : Controller
    {
        private readonly IDataService dataService;

        private readonly IHttpContextHelpers httpContextHelpers;

        private readonly IHttpHelpers httpHelpers;

        public PeersController(
            IDataService dataService,
            IHttpContextHelpers httpContextHelpers,
            IHttpHelpers httpHelpers)
        {
            this.dataService = dataService;

            this.httpContextHelpers = httpContextHelpers;

            this.httpHelpers = httpHelpers;
        }

        [HttpGet]
        public IActionResult Get()
        {
            List<string> peers = this.dataService.NodeInfo.PeersListUrls;

            return Ok(peers);
        }

        [HttpPost]
        public async Task<IActionResult> ConnectPeer([FromBody]Peer peer)
        {
            var currentNodePeers = this.dataService.NodeInfo.PeersListUrls;

            string currentNodeUrl = this.httpContextHelpers.GetApplicationUrl(HttpContext);

            if (currentNodePeers.Contains(peer.PeerUrl) || peer.PeerUrl == currentNodeUrl)
            {
                //return StatusCode(StatusCodes.Status409Conflict);
                return Ok(new { Message = string.Format("Peer already added: {0}", peer.PeerUrl) });
            }

            this.dataService.NodeInfo.PeersListUrls.Add(peer.PeerUrl);

            string peerPath = "peers";

            var currentNodePeer = new Peer()
            {
                PeerUrl = currentNodeUrl
            };

            var response = await this.httpHelpers.DoApiPost(peer.PeerUrl, peerPath, currentNodePeer);

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest($"{peer.PeerUrl} did not added successfully current peer, Status Code: {response.StatusCode}");
            }

            return Ok(new { Message = string.Format("Added peer: {0}", peer.PeerUrl) });
        }
    }
}
