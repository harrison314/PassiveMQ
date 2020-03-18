using Harrison314.PassiveMQ.Models;
using Harrison314.PassiveMQ.Services.Configuration;
using Harrison314.PassiveMQ.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Controllers
{
    [Route("api/[controller]")]
    [ProducesResponseType(typeof(ErrorResponseDto), 400)]
    [ProducesResponseType(typeof(ErrorResponseDto), 500)]
    public class MessagesController : Controller
    {
        private readonly IMessageRepository messageRepository;
        private readonly INotificationSender notificationSender;
        private readonly IOptions<MqSettings> mqSettings;
        private readonly ILogger<MessagesController> logger;

        public MessagesController(IMessageRepository messageRepository, INotificationSender notificationSender, IOptions<MqSettings> mqSettings, ILogger<MessagesController> logger)
        {
            this.messageRepository = messageRepository;
            this.notificationSender = notificationSender;
            this.mqSettings = mqSettings;
            this.logger = logger;
        }

        [HttpPost("{queuId}", Name = "CreateMessage")]
        [ProducesResponseType(typeof(MessageDto), 201)]
        public async Task<IActionResult> CreateMessage(Guid queuId, [FromBody]MessageCrateReqDto message)
        {
            this.logger.LogTrace("Call post with message label={0}", message.Label);

            Guid newMessageId = await this.messageRepository.CreateAsync(queuId, message).ConfigureAwait(false);
            await this.notificationSender.SendNotificationAsync(queuId, newMessageId).ConfigureAwait(false);
            MessageDto createdMessage = await this.messageRepository.ReadById(newMessageId).ConfigureAwait(false);

            return this.CreatedAtAction(nameof(this.Read), new { queuId = queuId, messageId = newMessageId }, createdMessage);
        }

        [HttpGet("{queuId}/message/{messageId}", Name = "ReadMessage")]
        [ProducesResponseType(typeof(MessageDto), 200)]
        public async Task<IActionResult> Read(Guid queuId, Guid messageId)
        {
            this.logger.LogTrace("Call Read, with messageId={0}", messageId);
            MessageDto message = await this.messageRepository.ReadById(messageId).ConfigureAwait(false);

            return this.Ok(message);
        }

        [HttpGet("{queuId}/peek", Name = "PeekMessage")]
        [ProducesResponseType(typeof(MessageDto), 200)]
        [ProducesResponseType(204)]
        public async Task<IActionResult> PeekMessage(Guid queuId)
        {
            this.logger.LogTrace("Call Peek for queuId={0}", queuId);
            MqSettings settings = this.mqSettings.Value;
            for (; ; )
            {
                Guid? messageId = await this.messageRepository.PeekMessageAsync(queuId).ConfigureAwait(false);

                if (messageId.HasValue)
                {
                    MessageDto message = await this.messageRepository.ReadById(messageId.Value).ConfigureAwait(false);
                    if (message.RetryCount > settings.MaxDefaultRetryCount)
                    {
                        await this.ProcessPosionMessage(message).ConfigureAwait(false);
                        await this.messageRepository.RemoveAsync(messageId.Value).ConfigureAwait(false);
                        continue;
                    }

                    return this.Ok(message);
                }
                else
                {
                    return this.NoContent();
                }
            }
        }

        [HttpGet("{queuId}/count", Name = "GetCount")]
        [ProducesResponseType(typeof(CountMessageDto), 200)]
        public async Task<IActionResult> GetCount(Guid queuId)
        {
            this.logger.LogTrace("Call GetCount for queuId={0}", queuId);

            CountMessageDto dto = new CountMessageDto();
            dto.QueuId = queuId;
            dto.Count = await this.messageRepository.GetCountAsync(queuId).ConfigureAwait(false);

            return this.Ok(dto);
        }

        [HttpPut("{queuId}/exchange", Name = "Exchange")]
        [ProducesResponseType(typeof(void), 204)]
        [ProducesResponseType(typeof(MessageDto), 200)]
        public async Task<IActionResult> Exchange(Guid queuId, [FromBody] ExchangeModel exchangeModel)
        {
            this.logger.LogTrace("Call GetMessage for queuId={0}, nextVisibleInMs={1}", queuId, exchangeModel?.NextVisibleInMs);

            MqSettings settings = this.mqSettings.Value;
            for (; ; )
            {
                TimeSpan nextVisibility = settings.DefaultRetry;
                if (exchangeModel != null && exchangeModel.NextVisibleInMs.HasValue)
                {
                    nextVisibility = TimeSpan.FromMilliseconds(exchangeModel.NextVisibleInMs.Value);
                }

                Guid? messageId = await this.messageRepository.ReserveMessageAsync(queuId, nextVisibility).ConfigureAwait(false);

                if (messageId.HasValue)
                {
                    MessageDto message = await this.messageRepository.ReadById(messageId.Value).ConfigureAwait(false);
                    if (message.RetryCount > settings.MaxDefaultRetryCount)
                    {
                        await this.ProcessPosionMessage(message).ConfigureAwait(false);
                        await this.messageRepository.RemoveAsync(messageId.Value).ConfigureAwait(false);
                        continue;
                    }

                    return this.Ok(message);
                }
                else
                {
                    return this.NoContent();
                }
            }
        }

        [HttpDelete("{queuId}/message/{messageId}", Name = "DeleteMessage")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteMessage(Guid queuId, Guid messageId)
        {
            this.logger.LogTrace("Delete message for queuId={0}, messageId={1}", queuId, messageId);
            await this.messageRepository.RemoveAsync(messageId).ConfigureAwait(false);

            return this.NoContent();
        }

        private Task ProcessPosionMessage(MessageDto message)
        {
            this.logger.LogError("Posison message:\n{0}", System.Text.Json.JsonSerializer.Serialize(message));
            return Task.CompletedTask;
        }
    }
}
