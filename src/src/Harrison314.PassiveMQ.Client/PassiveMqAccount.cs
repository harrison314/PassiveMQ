using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Harrison314.PassiveMQ.Client.Internal;

namespace Harrison314.PassiveMQ.Client
{
    public class PassiveMqAccount
    {
        private readonly string endpoint;
        private readonly HttpClient httpClient;

        internal PassiveMqAccount(string endpoint, HttpClient httpClient)
        {
            this.endpoint = endpoint;
            this.httpClient = httpClient;
        }

        public static PassiveMqAccount Parse(string connectionString, Func<HttpClient> httpClientFactory = null)
        {
            if (connectionString == null) throw new ArgumentNullException(nameof(connectionString));
            ConnectionString connection = new ConnectionString(connectionString);

            Uri endpoint = new Uri(connection.GetString("Endpoint"));

            HttpClient httpCLient = httpClientFactory == null ? HttpClientSinglton.Instance : httpClientFactory.Invoke();

            return new PassiveMqAccount(endpoint.OriginalString, httpCLient);
        }

        public static PassiveMqAccount Create(HttpClient httpClient)
        {
            if (httpClient == null) throw new ArgumentNullException(nameof(httpClient));

            return new PassiveMqAccount(null, httpClient);
        }

        public PassiveMqQueuClient CreateQueueClient(string queuName)
        {
            if (queuName == null) throw new ArgumentNullException(nameof(queuName));
            if (string.IsNullOrWhiteSpace(queuName)) throw new ArgumentException("Parameter queuName is empty or whitespace.");

            return new PassiveMqQueuClient(new PassiveMQAPI(this.httpClient),
                queuName);
        }

        public PassiveMqPublishClient CreatePublishClient()
        {
            return new PassiveMqPublishClient(new PassiveMQAPI(this.httpClient));
        }
    }
}
