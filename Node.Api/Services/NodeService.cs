using System;
using System.Collections.Generic;
using AutoMapper;
using Node.Api.Helpers;
using Node.Api.Models;
using Node.Api.Services.Abstractions;

namespace Node.Api.Services
{
    public class NodeService : INodeService
    {
        private readonly IMapper mapper;

        private readonly IDataService dataService;

        public NodeService(IMapper mapper, IDataService dataService)
        {
            this.mapper = mapper;

            this.dataService = dataService;
        }

        public TransactionSubmissionResponse AddTransaction(Transaction transaction)
        {
            bool signatureVerificationResult = this.VerifySignature(transaction);

            if (signatureVerificationResult == false)
            {
                throw new Exception("Signature verification not successful"); 
            }

            string transactionHash = this.CalculateTransactionHash(transaction);

            var transactionSubmissionResponse = new TransactionSubmissionResponse()
            {
                TransactionHash = transactionHash
            };

            return transactionSubmissionResponse;
        }

        public bool IsCollisionDetected(Transaction transaction)
        {
            return false;
        }

        private bool VerifySignature(Transaction transaction)
        {
            bool signatureVerificationResult;

            var transactionSignatureDataModel = mapper.Map<Transaction, TransactionSignatureDataModel>(transaction);

            // TODO: Verify signature using elliptic curve cryptography algorithm secp256k1 / ECDSA
            // BouncyCastle library can be used : ISigner - signer.VerifySignature()
            signatureVerificationResult = true;

            return signatureVerificationResult;
        }

        public void AddTransactionToPendingTransactions(Transaction transaction, List<Transaction> pendingTransactions)
        {
        }

        public void SendTransactionToPeerNodes(List<string> peerNodes)
        {
        }

        private string CalculateTransactionHash(Transaction transaction)
        {
            // Generating Date Time string using ISO8601 standard
            string transactionDateCreated = transaction.DateCreated.ToUniversalTime().ToString("o");

            string concatenatedTransactionPropertyValues =
                transaction.From + transaction.To +
                transaction.SenderPubKey + transaction.Value +
                transaction.Fee + transactionDateCreated + 
                transaction.SenderSignature[0] + transaction.SenderSignature[1];

            string transactionHash = Crypto.Sha256(concatenatedTransactionPropertyValues);

            return transactionHash;
        }
    }
}
