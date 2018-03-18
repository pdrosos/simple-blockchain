namespace Miner.Console
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.Security.Cryptography;
    using System.Text;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using Serilog;

    class Program
    {
        static string nodeUrl = "http://localhost:5555";
        static string minerAddress = "000000000";
           
        static void Main(string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine("Usage: \t dotnet Miner.Console.dll <NODE_URL> <Miner_Address>");

                return;
            }

            nodeUrl = args[0];
            minerAddress = args[1];

            var log = new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.File($"Log/Log-Miner-{minerAddress}.txt")
                .CreateLogger();

            log.Information("Hello, Serilog!");

            Mine();
        }

        public static void Mine()
        {
            string responseBody = @"{
                'blockIndex': 1,
                'transactionsIncluded': 12,
                'expectedReward': 6225,
                'rewardAddress': '1212121',
                'blockDataHash': '9f86d081884c7d659a2feaa0c55ad015a3bf4f1b2b0b822cd15d6c15b0f00a08',
                'difficulty': 5
            }";

            WebResponse response = null;
            HttpStatusCode statusCode = HttpStatusCode.RequestTimeout;

            Stopwatch sw = new Stopwatch();
            TimeSpan maxTaskLength = new TimeSpan(0, 0, 2); // 2 seconds

            while (true)
            {
                sw.Start();

                do
                {
                    try
                    {
                        statusCode = HttpStatusCode.RequestTimeout;

                        // Create a request to Node   
                        WebRequest request = WebRequest.Create(nodeUrl + "/mining/get-block/" + minerAddress);
                        request.Method = "GET";
                        request.Timeout = 3000;
                        request.ContentType = "application/json; charset=utf-8";

                        response = request.GetResponse();
                        statusCode = ((HttpWebResponse)response).StatusCode;
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

                Stream dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);
                responseBody = reader.ReadToEnd();

                reader.Close();
                dataStream.Close();
                response.Close();

                MiningJob miningJob = JsonConvert.DeserializeObject<MiningJob>(responseBody);

                Log.Information($"Successfully received mining job (Block Data Hash: {miningJob.BlockDataHash}) from node!");

                Console.WriteLine("Start New Mining Job:");
                Console.WriteLine("Block Index: {0}", miningJob.BlockIndex);
                Console.WriteLine("Transactions Included: {0}", miningJob.TransactionsIncluded);
                Console.WriteLine("Expected Reward: {0}", miningJob.ExpectedReward);
                Console.WriteLine("Reward Address: {0}", miningJob.RewardAddress);
                Console.WriteLine("Block Data Hash: {0}", miningJob.BlockDataHash);
                Console.WriteLine("Difficulty: {0}", miningJob.Difficulty);

                Boolean blockFound = false;
                UInt64 nonce = 0;
                String timestamp = DateTime.UtcNow.ToString("o");
                String difficulty = new String('0', miningJob.Difficulty) +
                    new String('9', 64 - miningJob.Difficulty);

                String blockData = miningJob.BlockIndex.ToString() + 
                                   miningJob.TransactionsIncluded.ToString() + 
                                   miningJob.BlockDataHash;
                String data;
                String blockHash;

                while (! blockFound && nonce < UInt32.MaxValue)
                {
                    data = blockData + timestamp + nonce.ToString();
                    blockHash = ByteArrayToHexString(Sha256(Encoding.UTF8.GetBytes(data)));
                    if (String.CompareOrdinal(blockHash, difficulty) < 0)
                    {
                        Console.WriteLine("Block Mined");
                        Console.WriteLine($"Block Hash: {blockHash}\n");

                        JObject obj = JObject.FromObject(new
                        {
                            nonce = nonce.ToString(),
                            dateCreated = timestamp,
                            blockHash = blockHash
                        });
                        byte[] blockFoundData = Encoding.UTF8.GetBytes(obj.ToString());

                        int retries = 0;
                        do
                        {
                            try
                            {
                                statusCode = HttpStatusCode.RequestTimeout;

                                WebRequest request = WebRequest.Create(nodeUrl + "/mining/get-block/" + minerAddress);
                                request.Method = "POST";
                                request.Timeout = 3000;
                                request.ContentType = "application/json; charset=utf-8";

                                dataStream = request.GetRequestStream();
                                dataStream.Write(blockFoundData, 0, blockFoundData.Length);
                                dataStream.Close();

                                Log.Information($"Sent request to: {nodeUrl} with mined block (hash: {blockHash})!");

                                response = request.GetResponse();
                                statusCode = ((HttpWebResponse)response).StatusCode;
                                string statusDescription = ((HttpWebResponse)response).StatusDescription;

                                Console.WriteLine(statusDescription);
                                Log.Information($"Received response from {nodeUrl} - statuc code: {statusCode}, description: {statusDescription}");

                                response.Close();
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
                        timestamp = DateTime.UtcNow.ToString("o");
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
