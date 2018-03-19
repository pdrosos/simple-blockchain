using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using AutoMapper;
using Swashbuckle.AspNetCore.Swagger;

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

            services.AddSingleton<ICryptographyHelpers, CryptographyHelpers>();

            services.AddSingleton<IDateTimeHelpers, DateTimeHelpers>();

            services.AddScoped<INodeService, NodeService>();

            services.AddScoped<IAddressService, AddressService>();

            services.AddScoped<IHttpContextHelpers, HttpContextHelpers>();

            services.AddScoped<IHttpHelpers, HttpHelpers>();
            
            // Register the Swagger generator, defining one or more Swagger documents
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Blockchain REST API", Version = "v1" });
                
                // Set the comments path for the Swagger JSON and UI.
                //var basePath = AppContext.BaseDirectory;
                //var xmlPath = Path.Combine(basePath, "Blockchain.xml");
                //c.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IDataService dataService, INodeService nodeService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            
            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                //c.RoutePrefix = "api-docs";
                c.DocumentTitle = "Simple Blockchain REST API";
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Simple Blockchain API V1");
            });

            app.UseCors("AllowAll");
            app.UseMvc();

            ApplicationSettings appSettings = this.Configuration.GetSection("App").Get<ApplicationSettings>();

            dataService.NodeInfo = new NodeInfo()
            {
                About = appSettings.About,
                Difficulty = appSettings.Difficulty,
                PeersListUrls = new List<string>()
            };

            dataService.MinerReward = appSettings.MinerReward;

            dataService.PendingTransactions = new List<Transaction>();

            dataService.Blocks = new List<Block>();

            nodeService.GenerateGenesisBlock();

            dataService.MiningJobs = new Dictionary<string, Block>();
        }
    }
}
