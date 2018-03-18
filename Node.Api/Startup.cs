using System.Collections.Generic;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using AutoMapper;

using Infrastructure.Library.Helpers;
using Node.Api.Configuration;
using Node.Api.Helpers;
using Node.Api.Models;
using Node.Api.Services;
using Node.Api.Services.Abstractions;


namespace Node.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                    });
            });

            services.AddMvc();

            services.AddAutoMapper();

            services.AddSingleton<IDataService, DataService>();

            services.AddSingleton<IMockedDataService, MockedDataService>();

            services.AddSingleton<ICryptographyHelpers, CryptographyHelpers>();

            services.AddSingleton<IDateTimeHelpers, DateTimeHelpers>();

            services.AddScoped<INodeService, NodeService>();

            services.AddScoped<IBlockService, BlockService>();

            services.AddScoped<ITransactionService, TransactionService>();

            services.AddScoped<IAddressService, AddressService>();

            services.AddScoped<IPeerService, PeerService>();

            services.AddScoped<IMiningService, MiningService>();

            services.AddScoped<IHttpContextHelpers, HttpContextHelpers>();

            services.AddScoped<IHttpHelpers, HttpHelpers>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IDataService dataService, INodeService nodeService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseCors("AllowAll");
            app.UseMvc();

            ApplicationSettings appSettings = this.Configuration.GetSection("App").Get<ApplicationSettings>();

            dataService.NodeInfo = new NodeInfo()
            {
                About = appSettings.About,
                Difficulty = appSettings.Difficulty,
                PeersListUrls = new List<string>()
            };

            dataService.PendingTransactions = new List<Transaction>();

            dataService.Blocks = new List<Block>();

            nodeService.GenerateGenesisBlock();

            dataService.MiningJobs = new Dictionary<string, Block>();
        }
    }
}
