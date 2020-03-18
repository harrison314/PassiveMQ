using Harrison314.PassiveMQ.Models;
using Harrison314.PassiveMQ.Services.Contracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
    public class PublishController : Controller
    {
        private readonly IMessageRepository messageRepository;
        private readonly INotificationSender notificationSender;
        private readonly ILogger<PublishController> logger;

        public PublishController(IMessageRepository messageRepository, INotificationSender notificationSender, ILogger<PublishController> logger)
        {
            this.messageRepository = messageRepository;
            this.notificationSender = notificationSender;
            this.logger = logger;
        }

        [HttpPost(Name = "Publish")]
        [ProducesResponseType(typeof(PublishResult), 201)]
        public async Task<IActionResult> PostPublish([FromBody] MessagePublishDto message)
        {
            this.logger.LogTrace("Call publis message fro topic: {0}", message.Topic);

            PublishResult result = await this.messageRepository.Publish(message.Topic, message.Message).ConfigureAwait(false);
            foreach (PublishPair storedMessage in result.CratedMessages)
            {
                await this.notificationSender.SendNotificationAsync(storedMessage.QueuId, storedMessage.MessageId).ConfigureAwait(false);
            }

            return this.CreatedAtAction(nameof(this.GetMessages), null, result);
        }

        [HttpGet(Name = "GetMessages")]
        [ProducesResponseType(typeof(void), 404)]
        public IActionResult GetMessages()
        {
            return this.NotFound();
        }
    }
}
