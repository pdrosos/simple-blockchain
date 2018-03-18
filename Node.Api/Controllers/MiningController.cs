namespace Node.Api.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Node.Api.Helpers;
    using Node.Api.Models;
    using Node.Api.Services.Abstractions;
    using System;

    [Route("[controller]")]
    public class MiningController : Controller
    {
        private readonly IDataService dataService;

        private readonly INodeService nodeService;

        public MiningController(IDataService dataService, INodeService nodeService)
        {
            this.dataService = dataService;

            this.nodeService = nodeService;
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

            this.nodeService.VerifyMinedJob(minedBlock);

            var response = new
            {
                Status = "accepted",
                Message = string.Format("Block accepted, reward paid.")
            };

            return Ok(response);
        }
    }
}
