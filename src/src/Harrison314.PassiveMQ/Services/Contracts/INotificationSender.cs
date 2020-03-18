using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Services.Contracts
{
    public interface INotificationSender
    {
        Task SendNotificationAsync(Guid queuId, Guid messageId);

        Task<bool> SetNotificationAdress(Guid queuId, string adress);
    }
}
