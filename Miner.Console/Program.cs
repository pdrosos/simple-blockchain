namespace Miner.Console
{
    using Infrastructure.Library.Helpers;
    using Microsoft.Extensions.DependencyInjection;
    using Miner.Console.Models;
    using Serilog;
    using Serilog.Core;
    using System;
    using System.Diagnostics;
    using System.Net;
    using System.Net.Http;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;

    class Program
    {
        static string nodeUrl = "http://localhost:5555";
        static string minerAddress = "000000000";
           
        static void Main(string[] args)
        {
            var services = new ServiceCollection();

            services.AddSingleton<IDateTimeHelpers, DateTimeHelpers>();

            services.AddScoped<IHttpHelpers, HttpHelpers>();

            var serviceProvider = services.BuildServiceProvider();

            var httpHelpers = serviceProvider.GetService<IHttpHelpers>();

            var dateTimeHelpers = serviceProvider.GetService<IDateTimeHelpers>();

            if (args.Length < 2)
            {
                Console.WriteLine("Usage: \t dotnet Miner.Console.dll <NODE_URL> <Miner_Address>");

                return;
            }

            nodeUrl = args[0];
            minerAddress = args[1];

            Console.WriteLine($"nodeUrl: {nodeUrl}");
            Console.WriteLine($"minerAddress: {minerAddress}");

            var log = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File($"Log/Log-Miner-{minerAddress}.txt")
                .CreateLogger();

            log.Information("Hello, Serilog!");

            MineAsync(httpHelpers, dateTimeHelpers, log).Wait();
        }

        public static async Task MineAsync(IHttpHelpers httpHelpers, IDateTimeHelpers dateTimeHelpers, Logger log)
        {
            HttpStatusCode statusCode = HttpStatusCode.RequestTimeout;

            Stopwatch sw = new Stopwatch();
            TimeSpan maxTaskLength = new TimeSpan(0, 0, 2); // 2 seconds

            while (true)
            {
                sw.Start();

                MiningJob miningJob = null;

                do
                {
                    try
                    {
                        string path = "mining/get-mining-job/{minerAddress}";

                        var parameter = new Parameter()
                        {
                            Name = "minerAddress",
                            Value = minerAddress,
                            Type = ParameterType.UrlSegment
                        };

                        log.Information($"Trying to get mining job from node: {nodeUrl}");

                        Response<MiningJob> response = await httpHelpers.DoApiGet<MiningJob>(nodeUrl, path, parameter);

                        miningJob = response.Data;

                        statusCode = response.StatusCode;
                    }
                    catch (WebException e)
                    {
                        Console.WriteLine("WebException raised!");
                        Console.WriteLine("{0}\n", e.Message);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Exception raised!");
                        Console.WriteLine("Source : {0}", e.Source);
                        Console.WriteLine("Message : {0}\n", e.Message);
                    }
                } while (statusCode != HttpStatusCode.OK);

                log.Information($"Successfully received mining job (Block Data Hash: {miningJob.BlockDataHash}) from node!");

                Console.WriteLine("Start New Mining Job:");
                Console.WriteLine("Block Index: {0}", miningJob.BlockIndex);
                Console.WriteLine("Transactions Included: {0}", miningJob.TransactionsIncluded);
                Console.WriteLine("Expected Reward: {0}", miningJob.ExpectedReward);
                Console.WriteLine("Reward Address: {0}", miningJob.RewardAddress);
                Console.WriteLine("Block Data Hash: {0}", miningJob.BlockDataHash);
                Console.WriteLine("Difficulty: {0}", miningJob.Difficulty);

                bool blockFound = false;
                ulong nonce = 0;
                string timestamp = dateTimeHelpers.ConvertDateTimeToUniversalTimeISO8601String(DateTime.Now);

                string difficulty = new String('0', miningJob.Difficulty) + new string('9', 64 - miningJob.Difficulty);

                string blockData = miningJob.BlockIndex.ToString() + 
                                   miningJob.TransactionsIncluded.ToString() + 
                                   miningJob.BlockDataHash;
                string data;
                string blockHash;

                while (!blockFound && nonce < uint.MaxValue)
                {
                    data = blockData + timestamp + nonce.ToString();
                    blockHash = ByteArrayToHexString(Sha256(Encoding.UTF8.GetBytes(data)));

                    if (String.CompareOrdinal(blockHash, difficulty) < 0)
                    {
                        Console.WriteLine("Block Mined");
                        Console.WriteLine($"Block Hash: {blockHash}\n");

                        var minedBlock = new MinedBlockPostModel()
                        {
                            DateCreated = timestamp,
                            Nonce = nonce,
                            BlockDataHash = blockHash
                        };

                        int retries = 0;
                        do
                        {
                            try
                            {
                                statusCode = HttpStatusCode.RequestTimeout;

                                string path = "mining/submit-mined-block";

                                HttpResponseMessage response = await httpHelpers.DoApiPost(nodeUrl, path, minedBlock);

                                statusCode = response.StatusCode;

                                string statusDescription = response.ReasonPhrase;

                                log.Information($"Sent request to: {nodeUrl} with mined block (hash: {blockHash})!");

                                Console.WriteLine(statusDescription);
                                log.Information($"Received response from {nodeUrl} - statuc code: {statusCode}, description: {statusDescription}");
                            }
                            catch (WebException e)
                            {
                                Console.WriteLine("WebException raised!");
                                Console.WriteLine("{0}\n", e.Message);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine("Exception raised!");
                                Console.WriteLine("Source : {0}", e.Source);
                                Console.WriteLine("Message : {0}\n", e.Message);
                            }

                            System.Threading.Thread.Sleep(1000);
                        } while (statusCode != HttpStatusCode.OK && retries++ < 3);

                        blockFound = true;
                        break;
                    }

                    // print intermediate data
                    if (nonce % 1000000 == 0)
                    {
                        Console.WriteLine(timestamp);
                        Console.WriteLine($"Nonce: {nonce}");
                        Console.WriteLine($"Block Hash: {blockHash}\n");
                    }

                    // get new timestamp on every 100000 iterations
                    if (nonce % 100000 == 0)
                    {
                        timestamp = dateTimeHelpers.ConvertDateTimeToUniversalTimeISO8601String(DateTime.Now);
                    }

                    nonce++;

                    if (maxTaskLength < sw.Elapsed)
                    {
                        sw.Reset();
                        break;
                    }
                }
            }
        }

        public static byte[] Sha256(byte[] array)
        {
            SHA256Managed hashstring = new SHA256Managed();
            
            return hashstring.ComputeHash(array);
        }

        public static string ByteArrayToHexString(byte[] bytes)
        {
            StringBuilder result = new StringBuilder(bytes.Length * 2);
            string hexAlphabet = "0123456789ABCDEF";

            foreach (byte b in bytes)
            {
                result.Append(hexAlphabet[(int)(b >> 4)]);
                result.Append(hexAlphabet[(int)(b & 0x0F)]);
            }

            return result.ToString().ToLower();
        }
    }
}
