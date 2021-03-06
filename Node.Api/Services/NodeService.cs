﻿using System;
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
using System.Text;

namespace Node.Api.Services
{
    public class NodeService : INodeService
    {
        private const string TransactionApiPath = @"transactions/send";
        private const string BlockNotifyApiPath = @"blocks/notify";

        private readonly IDataService dataService;

        private readonly IAddressService addressService;

        private readonly ICryptographyHelpers cryptographyHelpers;

        private readonly IDateTimeHelpers dateTimeHelpers;

        private readonly IHttpHelpers httpHelpers;

        private readonly ILogger<NodeService> logger;
        
        const string ZeroHash = "0000000000000000000000000000000000000000";

        const int OwnerInitialAmount = 10000000;

        const string OwnerAddress = "65339f3a4e26ca447a69fb2714d5337b7800bcb9";

        public NodeService(
            IDataService dataService,
            IAddressService addressService,
            ICryptographyHelpers cryptographyHelpers,
            IDateTimeHelpers dateTimeHelpers,
            IHttpHelpers httpHelpers,
            ILogger<NodeService> logger)
        {
            this.dataService = dataService;
            this.addressService = addressService;
            this.cryptographyHelpers = cryptographyHelpers;
            this.dateTimeHelpers = dateTimeHelpers;
            this.httpHelpers = httpHelpers;
            this.logger = logger;
        }

        public TransactionSubmissionResponse ProcessTransaction(Transaction transaction, string currentPeerUrl)
        {
            // verify signature
            bool signatureVerificationResult = this.VerifySignature(transaction);

            var transactionSubmissionResponse = new TransactionSubmissionResponse();

            if (signatureVerificationResult == false)
            {
                transactionSubmissionResponse.StatusCode = 400; // Bad request
                transactionSubmissionResponse.Message = "Signature verification failed";

                return transactionSubmissionResponse;
            }

            // check for collision
            string transactionHash = this.CalculateTransactionHash(transaction);

            var collisionDetected = this.IsCollisionDetected(
                transactionHash, 
                this.dataService.PendingTransactions = new List<Transaction>(), 
                this.dataService.Blocks
            );

            if (collisionDetected)
            {
                transactionSubmissionResponse.StatusCode = 409; //Conflict
                transactionSubmissionResponse.Message = "Collision has been detected";

                return transactionSubmissionResponse;
            }
            
            // validate balance
            var balance = this.addressService.GetAddressBalance(transaction.From);
            if (balance.ConfirmedBalance.BalanceValue < transaction.Value + transaction.Fee)
            {
                transactionSubmissionResponse.StatusCode = 422; //Unprocessable entity
                transactionSubmissionResponse.Message = "Insufficient funds";

                return transactionSubmissionResponse;
            }

            transaction.TransactionHash = transactionHash;

            transactionSubmissionResponse.TransactionHash = transactionHash;

            this.dataService.PendingTransactions.Add(transaction);

            this.logger.LogInformation($"Node: {currentPeerUrl} added transaction with hash: {transactionHash} to pending transactions");

            // send transaction to peers
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

            var miningJob = new MiningJob()
            {
                BlockIndex = blockCandidate.Index,
                TransactionsIncluded = blockCandidate.Transactions.Count,
                Difficulty = this.dataService.NodeInfo.Difficulty,
                ExpectedReward = this.dataService.MinerReward + blockCandidate.Transactions.Sum(t => t.Fee),
                RewardAddress = minerAddress,
                BlockDataHash = blockCandidate.BlockDataHash
            };

            return miningJob;
        }

        public void VerifyMinedJob(MinedBlockPostModel minedBlock)
        {
            var blockCandidate = this.dataService.MiningJobs[minedBlock.BlockDataHash];

            try
            {
                var blockHash = this.CalculateMinedBlockHash(blockCandidate, minedBlock);
                // if next block - add to blockchain and notify peers
                if (blockCandidate.Index == (ulong)this.dataService.Blocks.Count + 1)
                {
                    blockCandidate.Nonce = minedBlock.Nonce;
                    blockCandidate.BlockHash = blockHash;
                    blockCandidate.DateCreated = DateTime.Parse(minedBlock.DateCreated);

                    this.AddBlockToBlockchain(blockCandidate);
                    
                    //remove pending transactions that are already included in the blockchain
                    blockCandidate.Transactions.ForEach(transaction =>
                    {
                        this.dataService.PendingTransactions.Remove(
                            this.dataService
                                .PendingTransactions
                                .Single(t => t.TransactionHash == transaction.TransactionHash)
                        );
                    });

                    this.logger.LogInformation($"Block {blockCandidate.Index} with hash {blockHash} added to blockchain");
                }
                else
                {
                    this.logger.LogInformation($"Block candidate {blockCandidate.Index} with hash {blockHash} already exists in blockckain");
                }
            }
            catch (Exception e)
            {
                this.logger.LogInformation(e.Message);
            }
        }

        private Block CreateBlockCandidate(string minerAddress)
        {
            var latestBlock = this.dataService.Blocks.OrderByDescending(b => b.Index).FirstOrDefault();

            ulong blockCandidateIndex = (latestBlock != null ? latestBlock.Index + 1 : 1);

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
                PrevBlockHash = latestBlock.BlockHash,
                MinedBy = minerAddress
            };

            string blockCandidateJson = JsonConvert.SerializeObject(blockCandidate);

            string blockDataHash = this.cryptographyHelpers.CalcSHA256(blockCandidateJson);

            blockCandidate.BlockDataHash = blockDataHash;

            return blockCandidate;
        }

        private string CalculateMinedBlockHash(Block blockCandidate, MinedBlockPostModel minedBlock)
        {
            string blockData = blockCandidate.Index.ToString() +
                               blockCandidate.Transactions.Count.ToString() +
                               blockCandidate.BlockDataHash;
            var nonceStr = minedBlock.Nonce.ToString();
            
            var data = blockData + minedBlock.DateCreated + nonceStr;            
            var blockHash = this.cryptographyHelpers.CalcSHA256(data);
            
            string requiredLeadingZeroes = new String('0', blockCandidate.Difficulty);
            if (blockHash.StartsWith(requiredLeadingZeroes))
            {
                return blockHash;
            }

            throw new Exception(
                $"Can not calculate valid block {blockCandidate.Index} hash: hash {blockHash}, nonce {nonceStr}, difficulty {blockCandidate.Difficulty.ToString()}"
            );
        }

        public bool AddBlockToBlockchain(Block block)
        {
            if (this.dataService.Blocks != null && this.dataService.Blocks.Any(b => b.Index == block.Index))
            {
                return false;
            }

            this.dataService.Blocks.Add(block);

            // notify peers for new block
            if (block.Index > 1)
            {
                this.SendBlockToPeers(block);
            }

            return true;
        }

        public void ReceiveNewBlock(NewBlockNotification newBlockNotification)
        {
            // skip blocks from unknown peers
            if (!this.dataService.NodeInfo.PeersListUrls.Contains(newBlockNotification.Sender.PeerUrl))
            {
                this.logger.LogInformation(
                    $"Received block {newBlockNotification.Block.Index} from unknown peer {newBlockNotification.Sender.PeerUrl}, skipping..."
                );
                
                return;
            }
            
            var lastBlock = this.dataService.Blocks.Last();
            if (newBlockNotification.Block.Index <= lastBlock.Index)
            {
                // if we already have this block, ignore it
                this.logger.LogInformation($"Received existing block {newBlockNotification.Block.Index}, skipping...");
                
                return;
            }

            // todo: validate block's POW
//            if (!IsBlockValid(newBlockNotification.Block))
//            {
//                return;
//            }

            if (newBlockNotification.Block.Index == lastBlock.Index + 1 && 
                lastBlock.BlockHash == newBlockNotification.Block.PrevBlockHash)
            {
                // if this is the last block, add it to our own chain
                this.logger.LogInformation($"Received next block {newBlockNotification.Block.Index}, added to blockchain");
                
                this.dataService.Blocks.Add(newBlockNotification.Block);
            }
            else
            {
                Task.Run(async () =>
                {
                    // get peer chain and replace own chain with it
                    var peerBlocks = await this.httpHelpers.DoApiGet<List<Block>>(newBlockNotification.Sender.PeerUrl, "blocks");
                
                    // todo: validate block's POW
//                foreach (var block in peerBlocks.Data)
//                {
//                    
//                }

                    if (peerBlocks.Data.Count <= this.dataService.Blocks.Count)
                    {
                        // don't replace own blockchain with shorter peer blockchain
                        this.logger.LogInformation($"Peer blockchain <= own blockchain, skipping...");
                        
                        return;
                    }

                    lock (this.dataService.Blocks)
                    {
                        lock (this.dataService.PendingTransactions)
                        {
                            this.dataService.Blocks = peerBlocks.Data;
                            this.dataService.Blocks.ForEach(block =>
                            {
                                //remove pending transactions that are already included in the peer's blockchain
                                block.Transactions.ForEach(transaction =>
                                {
                                    this.dataService.PendingTransactions.Remove(
                                        this.dataService
                                            .PendingTransactions
                                            .Single(t => t.TransactionHash == transaction.TransactionHash)
                                    );
                                });
                            });
                        }
                    }
                });
            }
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
                To = OwnerAddress,
                From = ZeroHash,
                Value = OwnerInitialAmount,
                TransferSuccessful = true,
                DateCreated = DateTime.UtcNow,
                MinedInBlockIndex = 1,
                Fee = 0,
                TransactionHash = ZeroHash
            });

            this.AddBlockToBlockchain(new Block
            {
                Difficulty = this.dataService.NodeInfo.Difficulty,
                Index = 1,
                MinedBy = "Chuck Norris",
                PrevBlockHash = ZeroHash,
                BlockDataHash = ZeroHash,
                BlockHash = ZeroHash,
                Transactions = transaction,
                DateCreated = DateTime.UtcNow,
                Nonce = 0
            });
        }

        private void SendTransactionToPeers(Transaction transaction, string currentPeerUrl)
        {
            List<string> peers = this.dataService.NodeInfo.PeersListUrls;

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
                                this.logger.LogInformation($"Node: {peerUrl} successfully received transaction {transaction.TransactionHash}");
                            }
                            else
                            {
                                this.logger.LogInformation(
                                    $"Node: {peerUrl} could not receive transaction {transaction.TransactionHash} (Status Code: {response.StatusCode}, Reason: {response.ReasonPhrase})");
                            }
                            
                        })
                    );
                });

                Task.WaitAll(tasks.ToArray());
            }
        }

        private void SendBlockToPeers(Block block)
        {
            var tasks = new List<Task>();
            
            List<string> peers = this.dataService.NodeInfo.PeersListUrls;
            peers.ForEach(peerUrl =>
            {
                tasks.Add
                (
                    Task.Run(async() =>
                    {
                        var sender = new Peer();
                        sender.PeerUrl = this.dataService.NodeUrl;
                        var blockNotification = new NewBlockNotification
                        {
                            Block = block,
                            Sender = sender,
                        };
                        
                        this.logger.LogInformation($"Node: has sent block {block.Index} to: {peerUrl}");
                        var response = await this.httpHelpers.DoApiPost(peerUrl, BlockNotifyApiPath, blockNotification);

                        if (response.IsSuccessStatusCode)
                        {
                            this.logger.LogInformation($"Node: {peerUrl} successfully received block {block.Index}");
                        }
                        else
                        {
                            this.logger.LogInformation(
                                $"Node: {peerUrl} could not receive block {block.Index} (Status Code: {response.StatusCode}, Reason: {response.ReasonPhrase})");
                        }
                            
                    })
                );
            });

            Task.WaitAll(tasks.ToArray());
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
