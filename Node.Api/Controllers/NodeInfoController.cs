namespace Node.Api.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Node.Api.Helpers;
    using Node.Api.Models;
    using Node.Api.Services.Abstractions;

    [Route("info")]
    public class NodeInfoController : Controller
    {
        private readonly INodeService nodeService;

        private readonly IDataService dataService;

        private readonly IHttpContextHelpers httpContextHelpers;

        public NodeInfoController(
            IDataService dataService,
            INodeService nodeService,
            IHttpContextHelpers httpContextHelpers)
        {
            this.dataService = dataService;

            this.nodeService = nodeService;

            this.httpContextHelpers = httpContextHelpers;
        }

        // GET info
        [HttpGet]
        public IActionResult Get()
        {
            NodeInfo nodeInfo = this.dataService.NodeInfo;

            nodeInfo.NodeUrl = this.httpContextHelpers.GetApplicationUrl(HttpContext);

            return Ok(nodeInfo);
        }
    }
}
