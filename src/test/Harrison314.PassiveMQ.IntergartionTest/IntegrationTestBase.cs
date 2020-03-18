using Harrison314.PassiveMQ.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.IntergartionTest
{
    public abstract class IntegrationTestBase : IDisposable
    {
        private readonly TestServer server;
        private readonly HttpClient httpClient;
        private readonly PassiveMqAccount account;

        protected TestServer Server
        {
            get => this.server;
        }

        protected HttpClient HttpClient
        {
            get => this.httpClient;
        }

        protected PassiveMqAccount Account
        {
            get => this.account;
        }

        public IntegrationTestBase()
        {
            // Arrange
            Dictionary<string, string> configuration = new Dictionary<string, string>();
            configuration["ConnectionStrings:MsSqlDatabase"] = "Server=.\\SQLEXPRESS;Database=PassiveMqDb;Trusted_Connection=True;";

            ConfigurationBuilder builder = new ConfigurationBuilder();
            builder.AddInMemoryCollection(configuration);

            this.server = new TestServer(new WebHostBuilder()
                .UseEnvironment("Development")
                .UseConfiguration(builder.Build())
                .UseStartup<Startup>());
            this.httpClient = this.server.CreateClient();

            this.account = PassiveMqAccount.Create(this.httpClient);
        }

        public void Dispose()
        {
            this.server?.Dispose();
            this.httpClient?.Dispose();
        }
    }
}
