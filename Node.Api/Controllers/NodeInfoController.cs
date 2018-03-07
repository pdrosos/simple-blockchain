namespace Node.Api.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;

    using Node.Api.Models;
    using Node.Api.Services.Abstractions;

    [Route("info")]
    public class NodeInfoController : Controller
    {
        private readonly IMockedDataService mockedDataService;

        private readonly IConfiguration configuration;

        public NodeInfoController(IConfiguration configuration, IMockedDataService mockedDataService)
        {
            this.configuration = configuration;

            this.mockedDataService = mockedDataService;
        }

        // GET info
        [HttpGet]
        public IActionResult Get()
        {
            NodeInfo nodeInfo = this.mockedDataService.NodeInfo;

            nodeInfo.About = this.configuration["App:About"];

            nodeInfo.NodeUrl = this.configuration["App:NodeUrl"];

            nodeInfo.Difficulty = int.Parse(this.configuration["App:Difficulty"]);

            return Ok(nodeInfo);
        }
    }
}
