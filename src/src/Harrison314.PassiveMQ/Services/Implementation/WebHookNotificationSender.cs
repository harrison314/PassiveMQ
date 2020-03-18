using Harrison314.PassiveMQ.Services.Contracts;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Services.Implementation
{
    public abstract class WebHookNotificationSender : INotificationSender
    {

        private readonly ITimeAccessor timeAccessor;
        private readonly IMemoryCache memoryCache;
        private readonly ILogger logger;

        protected WebHookNotificationSender(ITimeAccessor timeAccessor, IMemoryCache memoryCache, ILoggerFactory loggerFactory)
        {
            this.timeAccessor = timeAccessor;
            this.memoryCache = memoryCache;
            this.logger = loggerFactory.CreateLogger(this.GetType());
        }

        public async Task SendNotificationAsync(Guid queuId, Guid messageId)
        {
            if (queuId == Guid.Empty) throw new ArgumentException("Parameter queuId is empty.");
            if (messageId == Guid.Empty) throw new ArgumentException("Parameter messageId is empty.");

            if (!this.memoryCache.TryGetValue($"WebHookNotification-{queuId}", out string url))
            {
                url = await this.ReadAdress(queuId).ConfigureAwait(false);
                this.memoryCache.Set($"WebHookNotification-{queuId}", url);
            }

            if (url != null)
            {
                this.logger.LogTrace("Prepare webhook on {0} for queuId={1} and messageId={2}.", url, queuId, messageId);

                string formatedUrl = url.Replace("$(queuId)", queuId.ToString(), StringComparison.Ordinal).Replace("${messageId}", messageId.ToString(), StringComparison.Ordinal);
                WebHookMessage message = new WebHookMessage()
                {
                    MessageId = messageId,
                    QueuId = queuId,
                    Time = this.timeAccessor.UtcNow
                };

                this.SuperssAwait(this.SendPostAsync(formatedUrl, message).ContinueWith(this.ContinueWebHook, formatedUrl));
            }
        }

        private async Task SendPostAsync(string formatedUrl, WebHookMessage message)
        {
            // TODO: Use HttpClientFactory
#warning use HTTP client factory
            using (HttpClient client = new HttpClient())
            {
                string convertedjson = System.Text.Json.JsonSerializer.Serialize(message);
                using (StringContent content = new StringContent(convertedjson, Encoding.UTF8, "application/json"))
                {
                    using (HttpResponseMessage response = await client.PostAsync(formatedUrl, content).ConfigureAwait(false))
                    {
                        string dataString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                        if (response.IsSuccessStatusCode)
                        {
                            this.logger.LogTrace("Succes web hook on {0} for queuId={1} and messageId={2} with content: {3}", formatedUrl, message.QueuId, message.MessageId, dataString);
                            this.logger.LogInformation("Succes web hook on {0} for queuId={1} and messageId={2}", formatedUrl, message.QueuId, message.MessageId);
                        }
                        else
                        {
                            this.logger.LogWarning("Failed web hook on {0} with status {1} for queuId={2} and messageId={3} with content: {4}",
                                formatedUrl,
                                message.QueuId,
                                message.MessageId,
                                response.StatusCode,
                                dataString);
                        }
                    }
                }
            }
        }

        public async Task<bool> SetNotificationAdress(Guid queuId, string adress)
        {
            if (queuId == Guid.Empty) throw new ArgumentException("Parameter queuId is empty.");

            bool hasUpdate = await this.SetNotificationAdressInternal(queuId, adress).ConfigureAwait(false);
            this.memoryCache.Remove($"WebHookNotification-{queuId}");

            return hasUpdate;
        }

        protected abstract Task<bool> SetNotificationAdressInternal(Guid queuId, string adress);

        protected abstract Task<string> ReadAdress(Guid queuId);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SuperssAwait(Task task)
        {
        }

        private void ContinueWebHook(Task continuedTask, object parameter)
        {
            if (continuedTask.Exception != null)
            {
                this.logger.LogError(continuedTask.Exception, "Error during send werbhook to url: {0}", parameter);
            }
        }
    }
}
