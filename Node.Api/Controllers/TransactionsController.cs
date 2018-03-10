﻿namespace Node.Api.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;

    using Node.Api.Models;
    using Node.Api.Services.Abstractions;

    [Route("[controller]")]
    public class TransactionsController : Controller
    {
        private readonly IMockedDataService mockedDataService;

        private readonly ITransactionService transactionService;

        public TransactionsController(IMockedDataService mockedDataService, ITransactionService transactionService)
        {
            this.mockedDataService = mockedDataService;

            this.transactionService = transactionService;
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
            IEnumerable<Transaction> pendingTransactions = this.mockedDataService.PendingTransactions;

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

            //TODO: Add transaction to pending transactions, send transaction to other nodes

            var transactionSubmissionResponse = new TransactionSubmissionResponse();

            return Ok(transactionSubmissionResponse);
        }
    }
}
