using Harrison314.PassiveMQ.Client.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Client
{
    public class PassiveMqPublishClient
    {
        private readonly IPassiveMQAPI passiveMqApi;

        internal PassiveMqPublishClient(IPassiveMQAPI passiveMqApi)
        {
            this.passiveMqApi = passiveMqApi;
        }

        public async Task Publish(string topic, MqCreateMessage message, CancellationToken cancellationToken = default)
        {
            if (topic == null) throw new ArgumentNullException(nameof(topic));
            if (message == null) throw new ArgumentNullException(nameof(message));

            Models.MessagePublishDto dto = new Models.MessagePublishDto()
            {
                Message = message.ToDto(),
                Topic = topic.Trim()
            };

            using (Microsoft.Rest.HttpOperationResponse<object> respone = await this.passiveMqApi.PublishWithHttpMessagesAsync(dto, null, cancellationToken).ConfigureAwait(false))
            {
                respone.AsResult();
            }
        }
    }
}
