using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Node.Api
{
    public class Program
    {
        public static int Main(string[] args)
        {
            string port = GetServerUrlPortNumber(args);

            string fileName = string.Empty;

            if (string.IsNullOrWhiteSpace(port))
            {
                fileName = $"Log/Log-Node.txt";
            }
            else
            {
                fileName = $"Log/Log-Node-{port}.txt";
            }

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.File(fileName, LogEventLevel.Information, fileSizeLimitBytes: null)
                .CreateLogger();

            try
            {
                Log.Information("Starting web host");
                BuildWebHost(args).Run();
                return 0;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
                return 1;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IWebHost BuildWebHost(string[] args)
        {
            var config = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();

            return WebHost.CreateDefaultBuilder(args)
                .UseConfiguration(config)
                .UseKestrel()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseSerilog()
                .Build();
        }

        private static string GetServerUrlPortNumber(string[] args)
        {
            // For getting command line args values consider "Command Line Parser Library for CLR and NetStandard":
            // https://github.com/commandlineparser/commandline

            string pattern = @"(?<port>:\d+)\s?";

            Regex myRegex = new Regex(pattern, RegexOptions.IgnoreCase);

            string port = string.Empty;

            foreach (var item in args)
            {
                if (item.Contains("--server.urls"))
                {
                    Match match = myRegex.Match(item);

                    if (match.Success)
                    {
                        port = match.Groups["port"].Value;

                        port = port.Replace(":", "");
                    }
                }
            }

            return port;
        }
    }
}
