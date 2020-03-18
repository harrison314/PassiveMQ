using Harrison314.PassiveMQ.Client.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Client
{
    public class PassiveMqQueuClient
    {
        private readonly IPassiveMQAPI passiveMqApi;
        private readonly string queuName;
        private Guid? queuId;

        internal PassiveMqQueuClient(IPassiveMQAPI passiveMqApi, string queuName)
        {
            this.passiveMqApi = passiveMqApi ?? throw new ArgumentNullException(nameof(passiveMqApi));
            this.queuName = queuName ?? throw new ArgumentNullException(nameof(queuName));
            this.queuId = null;
        }

        public async Task<bool> ExistsQueue(CancellationToken cancellationToken = default)
        {
            Guid? id = await this.GetQueuId(cancellationToken).ConfigureAwait(false);
            return id.HasValue;
        }

        public async Task CrateIfNotExists(string topicPattern = null, string notificationAdress = null, CancellationToken cancellationToken = default)
        {
            Models.QueuCreateReqDto dto = new Models.QueuCreateReqDto()
            {
                Name = this.queuName,
                TopicPattern = topicPattern,
                NotificationAdress = notificationAdress
            };

            using (Microsoft.Rest.HttpOperationResponse<object> response = await this.passiveMqApi.CreateQueuWithHttpMessagesAsync(false, dto, null, cancellationToken).ConfigureAwait(false))
            {
                response.AsResult();
            }
        }

        public async Task SetNotificationAdress(string notificationAdress, CancellationToken cancellationToken = default)
        {
            Guid? id = await this.GetQueuId().ConfigureAwait(false);
            if (id.HasValue)
            {
                Models.NotificationAdressUpdateDto notificationAdressUpdateDto = new Models.NotificationAdressUpdateDto()
                {
                    NotificationAdress = notificationAdress,
                    QueuId = id.Value
                };
                using (Microsoft.Rest.HttpOperationResponse<object> response = await this.passiveMqApi.PutNotificationAdressWithHttpMessagesAsync(id.Value, notificationAdressUpdateDto, null, cancellationToken).ConfigureAwait(false))
                {
                    response.AsResult();
                }
            }
            else
            {
                throw new PassiveMqException($"Queu with name {this.queuName} not found.");
            }
        }

        public async Task DeleteIfExists(CancellationToken cancellationToken = default)
        {
            Guid? id = await this.GetQueuId(cancellationToken).ConfigureAwait(false);
            if (id.HasValue)
            {
                using (Microsoft.Rest.HttpOperationResponse<Models.ErrorResponseDto> response = await this.passiveMqApi.DeleteQueuWithHttpMessagesAsync(id.Value, null, cancellationToken).ConfigureAwait(false))
                {
                    response.AsResult();
                    this.queuId = null;
                }
            }
        }

        public async Task<Queu> GetDefinition(CancellationToken cancellationToken = default)
        {
            using (Microsoft.Rest.HttpOperationResponse<object> response = await this.passiveMqApi.GetQueuByNameWithHttpMessagesAsync(this.queuName, null, cancellationToken).ConfigureAwait(false))
            {
                Models.QueuDto dto = response.AsResult<Models.QueuDto>();
                return Queu.FromDto(dto);
            }

        }

        public async Task<MqMessage> AddMessage(MqCreateMessage message, CancellationToken cancellationToken = default)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            Guid id = await this.EndshureQueuId(cancellationToken).ConfigureAwait(false);
            using (Microsoft.Rest.HttpOperationResponse<object> response = await this.passiveMqApi.CreateMessageWithHttpMessagesAsync(id, message.ToDto(), null, cancellationToken).ConfigureAwait(false))
            {
                Models.MessageDto dto = response.AsResult<Models.MessageDto>();
                return MqMessage.FromDto(dto);
            }
        }

        public async Task<int> GetCount(CancellationToken cancellationToken = default)
        {
            Guid id = await this.EndshureQueuId(cancellationToken).ConfigureAwait(false);
            using (Microsoft.Rest.HttpOperationResponse<object> response = await this.passiveMqApi.GetCountWithHttpMessagesAsync(id, null, cancellationToken).ConfigureAwait(false))
            {
                Models.CountMessageDto dto = response.AsResult<Models.CountMessageDto>();
                return dto.Count.Value;
            }
        }

        public async Task<MqMessage> GetMessage(System.TimeSpan? nextVisibilityTimeSpan = null, CancellationToken cancellationToken = default)
        {
            Guid id = await this.EndshureQueuId(cancellationToken).ConfigureAwait(false);

            Models.ExchangeModel model = new Models.ExchangeModel()
            {
                NextVisibleInMs = nextVisibilityTimeSpan.HasValue ? (int?)(nextVisibilityTimeSpan.Value.TotalMilliseconds) : null
            };

            using (Microsoft.Rest.HttpOperationResponse<object> response = await this.passiveMqApi.ExchangeWithHttpMessagesAsync(id, model, null, cancellationToken).ConfigureAwait(false))
            {
                Models.MessageDto dto = response.AsResult<Models.MessageDto>();
                return MqMessage.FromDto(dto);
            }
        }

        public async Task<MqMessage> PeekMessage(CancellationToken cancellationToken = default)
        {
            Guid id = await this.EndshureQueuId(cancellationToken).ConfigureAwait(false);

            using (Microsoft.Rest.HttpOperationResponse<object> response = await this.passiveMqApi.PeekMessageWithHttpMessagesAsync(id, null, cancellationToken).ConfigureAwait(false))
            {
                Models.MessageDto dto = response.AsResult<Models.MessageDto>();
                return MqMessage.FromDto(dto);
            }
        }

        public async Task DeleteMessage(Guid messageId, CancellationToken cancellationToken = default)
        {
            Guid id = await this.EndshureQueuId(cancellationToken).ConfigureAwait(false);
            using (Microsoft.Rest.HttpOperationResponse<Models.ErrorResponseDto> response = await this.passiveMqApi.DeleteMessageWithHttpMessagesAsync(id, messageId, null, cancellationToken).ConfigureAwait(false))
            {
                response.AsResult();
            }
        }

        public async Task DeleteMessage(MqMessage message, CancellationToken cancellationToken = default)
        {
            if (message == null) throw new ArgumentNullException(nameof(message));

            Guid id = await this.EndshureQueuId(cancellationToken).ConfigureAwait(false);
            using (Microsoft.Rest.HttpOperationResponse<Models.ErrorResponseDto> response = await this.passiveMqApi.DeleteMessageWithHttpMessagesAsync(id, message.Id, null, cancellationToken).ConfigureAwait(false))
            {
                response.AsResult();
            }
        }

        private async Task<Guid?> GetQueuId(CancellationToken cancellationToken = default)
        {
            using (Microsoft.Rest.HttpOperationResponse<object> response = await this.passiveMqApi.GetQueuByNameWithHttpMessagesAsync(this.queuName, null, cancellationToken).ConfigureAwait(false))
            {
                if (response.Response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }

                Models.QueuDto dto = response.AsResult<Models.QueuDto>();
                return dto.Id;
            }
        }

        private async Task<Guid> EndshureQueuId(CancellationToken cancellationToken = default)
        {
            if (!this.queuId.HasValue)
            {
                using (Microsoft.Rest.HttpOperationResponse<object> response = await this.passiveMqApi.GetQueuByNameWithHttpMessagesAsync(this.queuName, null, cancellationToken).ConfigureAwait(false))
                {
                    Models.QueuDto dto = response.AsResult<Models.QueuDto>();
                    this.queuId = dto.Id;
                }
            }

            return this.queuId.Value;
        }
    }
}
