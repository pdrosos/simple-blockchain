namespace Node.Api.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    using Node.Api.MockedData;
    using Node.Api.Models;

    [Route("api/[controller]")]
    public class MiningController : Controller
    {
        [HttpGet("get-mining-job/{minerAddress}")]
        public IActionResult GetBlock(string minerAddress)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            // TODO: Generate BlockDataHash from BlockCandidate, add the BlockCandidate to the MiningJobs dictionary
            // minerAddress - To field in the coinbase transaction

            MiningJob newMiningJob = MockedData.MiningJob;

            return Ok(newMiningJob);
        }

        [HttpPost("submit-mined-block")]
        public IActionResult SubmitBlock([FromBody]MinedBlock minedBlock)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
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
