using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
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
            services.AddMvc();

            services.AddSingleton<INodeService, NodeService>();

            services.AddSingleton<IDataService, DataService>();

            services.AddSingleton<IMockedDataService, MockedDataService>();

            services.AddScoped<IBlockService, BlockService>();

            services.AddScoped<ITransactionService, TransactionService>();

            services.AddScoped<IAddressService, AddressService>();

            services.AddScoped<IPeerService, PeerService>();

            services.AddScoped<IMiningService, MiningService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
