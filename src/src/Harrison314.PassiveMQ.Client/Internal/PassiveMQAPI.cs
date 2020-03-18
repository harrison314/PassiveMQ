using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Client
{
    public partial class PassiveMQAPI
    {
        public PassiveMQAPI(HttpClient httpClient)
            : base()
        {
            this.HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            this.BaseUri = this.HttpClient.BaseAddress;
        }
    }
}
