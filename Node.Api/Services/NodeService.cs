﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Org.BouncyCastle.Math;

using Node.Api.Helpers;
using Node.Api.Models;
using Node.Api.Services.Abstractions;

namespace Node.Api.Services
{
    public class NodeService : INodeService
    {
        const string TransactionApiPath = "transactions";

        private readonly IMapper mapper;

        private readonly IDataService dataService;

        private readonly ICryptographyHelpers cryptographyHelpers;

        private readonly IDateTimeHelpers dateTimeHelpers;

        private readonly IHttpHelpers httpHelpers;

        private readonly IHttpContextHelpers httpContextHelpers;

        public NodeService(
            IMapper mapper, 
            IDataService dataService, 
            ICryptographyHelpers cryptographyHelpers,
            IDateTimeHelpers dateTimeHelpers,
            IHttpHelpers httpHelpers,
            IHttpContextHelpers httpContextHelpers)
        {
            this.mapper = mapper;

            this.dataService = dataService;

            this.cryptographyHelpers = cryptographyHelpers;

            this.dateTimeHelpers = dateTimeHelpers;

            this.httpHelpers = httpHelpers;

            this.httpContextHelpers = httpContextHelpers;
        }

        public TransactionSubmissionResponse AddTransaction(Transaction transaction)
        {
            bool signatureVerificationResult = this.VerifySignature(transaction);

            var transactionSubmissionResponse = new TransactionSubmissionResponse();

            if (signatureVerificationResult == false)
            {
                transactionSubmissionResponse.StatusCode = 400; // Bad request
                transactionSubmissionResponse.Message = "Signature verification failed";

                return transactionSubmissionResponse;
            }

            string transactionHash = this.CalculateTransactionHash(transaction);

            var collisionDetected = this.IsCollisionDetected(transactionHash, this.dataService.PendingTransactions);

            if (collisionDetected)
            {
                transactionSubmissionResponse.StatusCode = 409;
                transactionSubmissionResponse.Message = "Collision has been detected";

                return transactionSubmissionResponse;
            }

            transaction.TransactionHash = transactionHash;

            this.dataService.PendingTransactions.Add(transaction);

            return transactionSubmissionResponse;
        }

        public void SendTransactionToPeers(Transaction transaction, string currentPeerUrl)
        {
            List<string> peers = this.dataService.NodeInfo.PeersListUrls;

            List<string> notYetSentToPeers = peers.Where(p => !transaction.AlreadySentToPeers.Any(url => url == p)).ToList();

            transaction.AlreadySentToPeers.AddRange(notYetSentToPeers);

            int sentToPeersCount = transaction.AlreadySentToPeers.Count;

            int storageLimit = 100;

            if (sentToPeersCount > storageLimit)
            {
                for (int i = sentToPeersCount - 1; i  >= storageLimit; i--)
                {
                    transaction.AlreadySentToPeers.RemoveAt(i);
                }
            }

            var tasks = new List<Task>();

            notYetSentToPeers.ForEach(peerUrl =>
            {
                tasks.Add(Task.Run(() => this.httpHelpers.DoApiPost(peerUrl, TransactionApiPath, transaction)));
            });

            Task.WaitAll(tasks.ToArray());
        }

        public bool IsCollisionDetected(string transactionHash, List<Transaction> pendingTransactions)
        {
            bool collisionDetected = pendingTransactions.Any(t => t.TransactionHash == transactionHash);

            return collisionDetected;
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

            byte[] publicKeyBytes = this.cryptographyHelpers.ConvertHexStringToByteArray(transaction.SenderPubKey);
            BigInteger[] signatureBigInteger = this.cryptographyHelpers.ConvertHexSignatureToBigInteger(transaction.SenderSignature);
            byte[] messageBytes = this.cryptographyHelpers.CalcSHA256BytesArray(transactionSignatureDataModelJson);

            signatureVerificationResult = this.cryptographyHelpers.VerifySignatureUsingSecp256k1(
                publicKeyBytes,
                signatureBigInteger,
                messageBytes
            );

            return signatureVerificationResult;
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
