using System;
using System.Collections.Generic;
using AutoMapper;
using Node.Api.Helpers;
using Node.Api.Models;
using Node.Api.Services.Abstractions;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Node.Api.Services
{
    public class NodeService : INodeService
    {
        private readonly IMapper mapper;

        private readonly IDataService dataService;

        private readonly ICryptographyHelpers cryptographyHelpers;

        private readonly IDateTimeHelpers dateTimeHelpers;

        public NodeService(
            IMapper mapper, 
            IDataService dataService, 
            ICryptographyHelpers cryptographyHelpers,
            IDateTimeHelpers dateTimeHelpers)
        {
            this.mapper = mapper;

            this.dataService = dataService;

            this.cryptographyHelpers = cryptographyHelpers;

            this.dateTimeHelpers = dateTimeHelpers;
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

            var transactionSignatureDataModel = new TransactionSignatureDataModel
            {
                From = transaction.From,
                To = transaction.To,
                SenderPubKey = transaction.SenderPubKey,
                Value = transaction.Value,
                Fee = transaction.Fee,
                DateCreated = this.dateTimeHelpers.ConvertDateTimeToUniversalTimeISO8601String(transaction.DateCreated)
            };

            DefaultContractResolver contractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            };

            var jsonSerializerSettings = new JsonSerializerSettings
            {
                ContractResolver = contractResolver,
                Formatting = Formatting.None
            };

            string transactionSignatureDataModelJson = 
                JsonConvert.SerializeObject(transactionSignatureDataModel, jsonSerializerSettings);

            signatureVerificationResult = this.cryptographyHelpers.VerifySignatureUsingSecp256k1(
                transaction.SenderPubKey, 
                transaction.SenderSignature, 
                transactionSignatureDataModelJson);

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
            string concatenatedTransactionProperties = this.ConcatenateTransactionProperties(transaction);

            string transactionHash = this.cryptographyHelpers.CalcSHA256(concatenatedTransactionProperties);

            return transactionHash;
        }

        private string ConcatenateTransactionProperties(Transaction transaction)
        {
            string transactionDateCreated = this.dateTimeHelpers.ConvertDateTimeToUniversalTimeISO8601String(transaction.DateCreated);

            string concatenatedTransactionProperties =
                transaction.From + transaction.To +
                transaction.SenderPubKey + transaction.Value +
                transaction.Fee + transactionDateCreated +
                transaction.SenderSignature[0] + transaction.SenderSignature[1];

            return concatenatedTransactionProperties;
        }
    }
}
