namespace Node.Api.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Node.Api.MockedData;
    using Node.Api.Models;
    
    [Route("api/info")]
    public class NodeInfoController : Controller
    {
        private readonly IConfiguration configuration;

        public NodeInfoController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpGet]
        public IActionResult  Get()
        {
            NodeInfo nodeInfo = MockedData.NodeInfo;

            nodeInfo.About = this.configuration["App:About"];

            nodeInfo.NodeUrl = this.configuration["App:NodeUrl"];

            nodeInfo.Difficulty = int.Parse(this.configuration["App:Difficulty"]);

            return Ok(nodeInfo);
        }
    }
}
