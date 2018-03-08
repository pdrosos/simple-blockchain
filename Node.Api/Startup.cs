﻿using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Node.Api.Configuration;
using Node.Api.Models;
using Node.Api.Services;
using Node.Api.Services.Abstractions;
using Node.Api.Helpers;

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
            services.AddMvc();

            services.AddSingleton<INodeService, NodeService>();

            services.AddSingleton<IDataService, DataService>();

            services.AddSingleton<IMockedDataService, MockedDataService>();

            services.AddScoped<IHttpContextHelpers, HttpContextHelpers>();

            services.AddScoped<IBlockService, BlockService>();

            services.AddScoped<ITransactionService, TransactionService>();

            services.AddScoped<IAddressService, AddressService>();

            services.AddScoped<IPeerService, PeerService>();

            services.AddScoped<IMiningService, MiningService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IDataService dataService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();

            ApplicationSettings appSettings = this.Configuration.GetSection("App").Get<ApplicationSettings>();

            dataService.NodeInfo = new NodeInfo()
            {
                About = appSettings.About,
                Difficulty = appSettings.Difficulty,
                PeersListUrls = new List<string>()
            };
        }
    }
}
