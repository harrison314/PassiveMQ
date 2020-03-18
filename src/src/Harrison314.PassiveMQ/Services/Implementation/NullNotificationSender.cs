using Harrison314.PassiveMQ.Services.Contracts;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Services.Implementation
{
    public class NullNotificationSender : INotificationSender
    {
        private readonly ILogger<NullNotificationSender> logger;

        public NullNotificationSender(ILogger<NullNotificationSender> logger)
        {
            this.logger = logger;
        }

        public Task SendNotificationAsync(Guid queuId, Guid messageId)
        {
            this.logger.LogInformation("Send notofication dummy for queuId={0}, messageId={1}", queuId, messageId);
            return Task.CompletedTask;
        }

        public Task<bool> SetNotificationAdress(Guid queuId, string adress)
        {
            this.logger.LogInformation("Set noticiation adress for queuId={0}, adress='{1}'", queuId, adress);
            return Task.FromResult(true);
        }
    }
}
