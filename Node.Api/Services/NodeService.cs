using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Org.BouncyCastle.Math;

using Infrastructure.Library.Helpers;
using Node.Api.Helpers;
using Node.Api.Models;
using Node.Api.Services.Abstractions;

namespace Node.Api.Services
{
    public class NodeService : INodeService
    {
        const string TransactionApiPath = @"transactions/send";

        private readonly IMapper mapper;

        private readonly IDataService dataService;

        private readonly IMockedDataService mockedDataService;

        private readonly ICryptographyHelpers cryptographyHelpers;

        private readonly IDateTimeHelpers dateTimeHelpers;

        private readonly IHttpHelpers httpHelpers;

        private readonly IHttpContextHelpers httpContextHelpers;

        private readonly ILogger<NodeService> logger;

        const int FaucetStartVolume = 1000000;

        const string ZeroHash = "0000000000000000000000000000000000000000";

        const string FaucetAddress = "a4a239576a1d25b32cf2a037e3540f6a2326fdc3";

        public NodeService(
            IMapper mapper, 
            IDataService dataService,
            IMockedDataService mockedDataService,
            ICryptographyHelpers cryptographyHelpers,
            IDateTimeHelpers dateTimeHelpers,
            IHttpHelpers httpHelpers,
            IHttpContextHelpers httpContextHelpers,
            ILogger<NodeService> logger)
        {
            this.mapper = mapper;

            this.dataService = dataService;

            this.mockedDataService = mockedDataService;

            this.cryptographyHelpers = cryptographyHelpers;

            this.dateTimeHelpers = dateTimeHelpers;

            this.httpHelpers = httpHelpers;

            this.httpContextHelpers = httpContextHelpers;

            this.logger = logger;
        }

        public TransactionSubmissionResponse ProcessTransaction(Transaction transaction, string currentPeerUrl)
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

            var collisionDetected = this.IsCollisionDetected(
                transactionHash, 
                this.dataService.PendingTransactions = new List<Transaction>(), 
                this.dataService.Blocks = new List<Block>());

            if (collisionDetected)
            {
                transactionSubmissionResponse.StatusCode = 409; //Conflict
                transactionSubmissionResponse.Message = "Collision has been detected";

                return transactionSubmissionResponse;
            }

            transaction.TransactionHash = transactionHash;

            transactionSubmissionResponse.TransactionHash = transactionHash;

            this.dataService.PendingTransactions.Add(transaction);

            this.logger.LogInformation($"Node: {currentPeerUrl} added transaction with hash: {transactionHash} to pending transactions");

            this.SendTransactionToPeers(transaction, currentPeerUrl);

            return transactionSubmissionResponse;
        }

        public MiningJob GetMiningJob(string minerAddress)
        {
            Block blockCandidate = this.CreateBlockCandidate(minerAddress);

            if (blockCandidate == null)
            {
                throw new Exception("Cannot create block candidate");
            }

            bool additionSuccessful = this.dataService.MiningJobs.TryAdd(blockCandidate.BlockDataHash, blockCandidate);

            if (!additionSuccessful)
            {
                throw new Exception("Adding new mining job to the list was not successful");
            }

            return this.mockedDataService.MiningJob;
        }

        private Block CreateBlockCandidate(string minerAddress)
        {
            var latestBlock = this.dataService.Blocks.OrderByDescending(b => b.Index).FirstOrDefault();

            long blockCandidateIndex = (latestBlock != null ? latestBlock.Index + 1 : 1);

            var coinbaseTransaction = new Transaction()
            {
                From = new string('0', 64),
                To = minerAddress,
                Value = this.dataService.MinerReward,
                Fee = 0,
                DateCreated = DateTime.Now,
                SenderPubKey = new string('0', 64),
                SenderSignature = new string[] { new string('0', 64) , new string('0', 64) },
                MinedInBlockIndex = blockCandidateIndex,
                TransferSuccessful = true
            };

            coinbaseTransaction.TransactionHash = this.CalculateTransactionHash(coinbaseTransaction);

            List<Transaction> blockCandidateTransactions = new List<Transaction>(this.dataService.PendingTransactions);

            blockCandidateTransactions.Insert(0, coinbaseTransaction);

            var blockCandidate = new Block()
            {
                Index = blockCandidateIndex,
                Transactions = blockCandidateTransactions,
                Difficulty = this.dataService.NodeInfo.Difficulty,
                PrevBlockHash = latestBlock.BlockDataHash,
                MinedBy = minerAddress
            };

            string blockCandidateJson = JsonConvert.SerializeObject(blockCandidate);

            string blockDataHash = this.cryptographyHelpers.CalcSHA256(blockCandidateJson);

            blockCandidate.BlockDataHash = blockDataHash;

            return blockCandidate;
        }

        public bool AddBlockToBlockchain(Block block)
        {
            if (this.dataService.Blocks != null && this.dataService.Blocks.Any(b => b.Index == block.Index))
            {
                return false;
            }

            this.dataService.Blocks.Add(block);

            //TODO: Notify peers for block

            return true;
        }

        public void GenerateGenesisBlock()
        {
            if (this.dataService.Blocks != null && this.dataService.Blocks.Count > 0)
            {
                return;
            }

            var transaction = new List<Transaction>();

            transaction.Add(new Transaction
            {
                To = FaucetAddress,
                From = ZeroHash,
                Value = FaucetStartVolume,
                TransferSuccessful = true,
                DateCreated = DateTime.UtcNow,
                MinedInBlockIndex = 0,
                Fee = 0,
                TransactionHash = ZeroHash
            });

            this.AddBlockToBlockchain(new Block
            {
                Difficulty = this.dataService.NodeInfo.Difficulty,
                Index = 0,
                MinedBy = "Michael Jordan",
                PrevBlockHash = ZeroHash,
                BlockDataHash = ZeroHash,
                Transactions = transaction,
                DateCreated = DateTime.UtcNow,
                Nonce = 0
            });
        }

        private void SendTransactionToPeers(Transaction transaction, string currentPeerUrl)
        {
            List<string> peers = this.dataService.NodeInfo.PeersListUrls;

            if (peers == null)
            {
                peers = new List<string>();
            }

            if (transaction.AlreadySentToPeers == null)
            {
                transaction.AlreadySentToPeers = new List<string>();
            }

            List<string> notYetSentToPeers = peers.Where(p => !transaction.AlreadySentToPeers.Any(url => url == p)).ToList();

            int sentToPeersCount = transaction.AlreadySentToPeers.Count;

            const int storageLimit = 100;

            if (sentToPeersCount > storageLimit)
            {
                int peersForRemovalCount = sentToPeersCount - storageLimit;

                for (int i = 0; i  < peersForRemovalCount; i++)
                {
                    transaction.AlreadySentToPeers.RemoveAt(i);
                }
            }

            if (notYetSentToPeers != null && notYetSentToPeers.Count > 0)
            {
                transaction.AlreadySentToPeers.AddRange(notYetSentToPeers);

                transaction.AlreadySentToPeers.Add(currentPeerUrl);

                var tasks = new List<Task>();

                notYetSentToPeers.ForEach(peerUrl =>
                {
                    tasks.Add
                    (
                        Task.Run(async() =>
                        {
                            this.logger.LogInformation($"Node: {currentPeerUrl} has sent transaction to: {peerUrl}");
                            var response = await this.httpHelpers.DoApiPost(peerUrl, TransactionApiPath, transaction);

                            if (response.IsSuccessStatusCode)
                            {
                                this.logger.LogInformation($"Node: {peerUrl} successfully received the transaction");
                            }
                            else
                            {
                                this.logger.LogInformation(
                                    $"Node: {peerUrl} could not receive transaction (Status Code: {response.StatusCode}, Reason: {response.ReasonPhrase})");
                            }
                            
                        })
                    );
                });

                Task.WaitAll(tasks.ToArray());
            }
        }

        private bool IsCollisionDetected(string transactionHash, List<Transaction> pendingTransactions, List<Block> blocks)
        {
            bool collisionDetected = pendingTransactions.Any(t => t.TransactionHash == transactionHash) ||
                blocks.Any(b => b.Transactions.Any(t => t.TransactionHash == transactionHash));

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
