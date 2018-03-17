namespace Node.Api.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    using Node.Api.Helpers;
    using Node.Api.Models;
    using Node.Api.Services.Abstractions;

    [Route("[controller]")]
    public class TransactionsController : Controller
    {
        private readonly IDataService dataService;

        private readonly IMockedDataService mockedDataService;

        private readonly INodeService nodeService;

        private readonly ITransactionService transactionService;

        private readonly IHttpContextHelpers httpContextHelpers;

        public TransactionsController(
            IDataService dataService, 
            IMockedDataService mockedDataService, 
            ITransactionService transactionService, 
            INodeService nodeService,
            IHttpContextHelpers httpContextHelpers)
        {
            this.dataService = dataService;

            this.mockedDataService = mockedDataService;

            this.transactionService = transactionService;

            this.nodeService = nodeService;

            this.httpContextHelpers = httpContextHelpers;
        }

        // GET transactions/23fe06345cc864aed086465ff8276c9ec3ac267
        [HttpGet("{transactionHash}")]
        public IActionResult GetTransactionInfo(string transactionHash)
        {
            var transaction = this.mockedDataService.Blocks
                .Select(b => b.Transactions.FirstOrDefault(tr => tr.TransactionHash == transactionHash))
                .FirstOrDefault();

            if (transaction == null)
            {
                return NotFound();
            }

            return Ok(transaction);
        }

        // GET transactions/confirmed
        [HttpGet("confirmed")]
        public IActionResult GetConfirmedTransactions()
        {
            IEnumerable<Transaction> transactions = this.mockedDataService.Blocks
                .SelectMany(b => b.Transactions)
                .Where(t => t.TransferSuccessful == true);

            return Ok(transactions);
        }

        // GET transactions/pending
        [HttpGet("pending")]
        public IActionResult GetPendingTransactions()
        {
            IEnumerable<Transaction> pendingTransactions = this.dataService.PendingTransactions;

            return Ok(pendingTransactions);
        }

        // POST transactions/send
        [HttpPost("send")]
        public IActionResult SendTransaction([FromBody]Transaction transaction)
        {
            if (!ModelState.IsValid)
            {
                ModelError firstModelError = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .FirstOrDefault();

                return BadRequest(new { ErrorMsg = firstModelError.ErrorMessage });
            }

            string applicationUrl = this.httpContextHelpers.GetApplicationUrl(HttpContext);

            TransactionSubmissionResponse transactionSubmissionResponse = 
                this.nodeService.ProcessTransaction(transaction, applicationUrl);

            if (transactionSubmissionResponse.StatusCode != null)
            {
                return StatusCode(
                    (int)transactionSubmissionResponse.StatusCode, 
                    new { ErrorMessage = transactionSubmissionResponse.Message });
            }

            return Ok(new { TransactionHash = transactionSubmissionResponse.TransactionHash });
        }
    }
}
