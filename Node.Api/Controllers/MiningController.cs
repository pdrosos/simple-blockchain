namespace Node.Api.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Node.Api.Models;
    using Node.Api.Services.Abstractions;
    using System;

    [Route("[controller]")]
    public class MiningController : Controller
    {
        private readonly IMockedDataService mockedDataService;

        private readonly IDataService dataService;

        private readonly INodeService nodeService;

        private readonly IMiningService miningService;

        private readonly ILogger<MiningController> logger;

        public MiningController(
            IDataService dataService, 
            IMockedDataService mockedDataService,
            INodeService nodeService,
            IMiningService miningService, 
            ILogger<MiningController> logger)
        {
            this.dataService = dataService;

            this.mockedDataService = mockedDataService;

            this.nodeService = nodeService;

            this.miningService = miningService;

            this.logger = logger;
        }

        [HttpGet("get-mining-job/{minerAddress}")]
        public IActionResult GetBlock(string minerAddress)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            // TODO: Generate BlockDataHash from BlockCandidate, add the BlockCandidate to the MiningJobs dictionary
            // minerAddress - To field in the coinbase transaction

            MiningJob miningJob = null;

            try
            {
                miningJob = this.nodeService.GetMiningJob(minerAddress);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok(miningJob);
        }

        [HttpPost("submit-mined-block")]
        public IActionResult SubmitBlock([FromBody]MinedBlockPostModel minedBlock)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (!this.dataService.MiningJobs.ContainsKey(minedBlock.BlockDataHash))
            {
                return NotFound();
            }

            var reward = 5000350;

            var response = new
            {
                Status = "accepted",
                Message = string.Format("Block accepted, reward paid: {0} microcoins", reward)
            };

            return Ok(response);
        }
    }
}
