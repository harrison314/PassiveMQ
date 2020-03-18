using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Client.Internal
{
    internal static class HttpClientSinglton
    {
        private static HttpClient httpClient = null;

        public static HttpClient Instance
        {
            get
            {
                if (httpClient == null)
                {
                    lock (typeof(HttpClientSinglton))
                    {
                        if (httpClient == null)
                        {
                            httpClient = new HttpClient();
                        }
                    }
                }

                return httpClient;
            }
        }

    }
}
