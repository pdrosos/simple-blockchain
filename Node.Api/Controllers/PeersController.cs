namespace Node.Api.Controllers
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;

    using Node.Api.Models;
    using Microsoft.AspNetCore.Http;
    using Node.Api.Services.Abstractions;
    using System.Net.Http.Headers;
    using System.Net.Http;
    using Node.Api.Extensions;
    using Node.Api.Helpers;

    [Route("[controller]")]
    public class PeersController : Controller
    {
        private readonly IDataService dataService;

        private readonly IMockedDataService mockedDataService;

        private readonly IPeerService peerService;

        private readonly IHttpContextHelpers httpContextHelpers;

        public PeersController(
            IDataService dataService, 
            IMockedDataService mockedDataService, 
            IPeerService peerService,
            IHttpContextHelpers httpContextHelpers)
        {
            this.dataService = dataService;

            this.mockedDataService = mockedDataService;

            this.peerService = peerService;

            this.httpContextHelpers = httpContextHelpers;
        }

        [HttpGet]
        public IActionResult Get()
        {
            List<string> peers = this.dataService.NodeInfo.PeersListUrls;

            return Ok(peers);
        }

        [HttpPost]
        public IActionResult ConnectPeer([FromBody]Peer peer)
        {
            var currentNodePeers = this.dataService.NodeInfo.PeersListUrls;

            string currentNodeUrl = this.httpContextHelpers.GetApplicationUrl(HttpContext);

            if (currentNodePeers.Contains(peer.PeerUrl) || peer.PeerUrl == currentNodeUrl)
            {
                return StatusCode(StatusCodes.Status409Conflict);
            }

            this.dataService.NodeInfo.PeersListUrls.Add(peer.PeerUrl);

            var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Accept.Clear();

            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var currentNodePeer = new Peer()
            {
                PeerUrl = currentNodeUrl
            };

            string requestUrl = $"{peer.PeerUrl}/peers";

            var response = httpClient.PostAsync(requestUrl, new JsonContent(currentNodePeer));

            return Ok(new { Message = string.Format("Added peer: {0}", peer.PeerUrl) });
        }
    }
}
