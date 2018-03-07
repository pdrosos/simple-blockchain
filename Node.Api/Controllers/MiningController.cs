namespace Node.Api.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using Node.Api.Models;
    using Node.Api.Services.Abstractions;

    [Route("[controller]")]
    public class MiningController : Controller
    {
        private readonly IMockedDataService mockedDataService;

        private readonly IMiningService miningService;

        public MiningController(IMockedDataService mockedDataService, IMiningService miningService)
        {
            this.mockedDataService = mockedDataService;

            this.miningService = miningService;
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

            MiningJob newMiningJob = this.mockedDataService.MiningJob;

            return Ok(newMiningJob);
        }

        [HttpPost("submit-mined-block")]
        public IActionResult SubmitBlock([FromBody]MinedBlock minedBlock)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (!this.mockedDataService.MiningJobs.ContainsKey(minedBlock.BlockDataHash))
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
