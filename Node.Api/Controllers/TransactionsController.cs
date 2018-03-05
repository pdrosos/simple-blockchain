namespace Node.Api.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    using Node.Api.Models;
    using Node.Api.MockedData;

    [Route("api/[controller]")]
    public class TransactionsController : Controller
    {
        // GET api/transactions/23fe06345cc864aed086465ff8276c9ec3ac267
        [HttpGet("{transactionHash}")]
        public IActionResult GetTransactionInfo(string transactionHash)
        {
            var transaction = MockedData.Blocks
                .Select(b => b.Transactions.FirstOrDefault(tr => tr.TransactionHash == transactionHash))
                .FirstOrDefault();

            if (transaction == null)
            {
                return NotFound();
            }

            return Ok();
        }

        // GET api/transactions/confirmed
        [HttpGet("confirmed")]
        public IActionResult GetConfirmedTransactions()
        {
            IEnumerable<Transaction> transactions = MockedData.Blocks
                .SelectMany(b => b.Transactions)
                .Where(t => t.TransferSuccessful == true);

            return Ok(transactions);
        }

        // GET api/transactions/pending
        [HttpGet("pending")]
        public IActionResult GetPendingTransactions()
        {
            IEnumerable<Transaction> pendingTransactions = MockedData.PendingTransactions;

            return Ok(pendingTransactions);
        }

        // GET api/transactions/send
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

            //TODO: Add transaction to pending transactions, send transaction to other nodes

            return Ok();
        }
    }
}
