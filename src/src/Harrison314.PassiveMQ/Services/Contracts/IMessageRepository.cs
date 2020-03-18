using Harrison314.PassiveMQ.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Services.Contracts
{
    public interface IMessageRepository
    {
        Task<Guid> CreateAsync(Guid queuId, MessageCrateReqDto message);

        Task<MessageDto> ReadById(Guid newMessageId);

        Task<int> GetCountAsync(Guid queuId);

        Task<Guid?> ReserveMessageAsync(Guid queuId, TimeSpan nextVisibility);

        Task<Guid?> PeekMessageAsync(Guid queuId);

        Task RemoveAsync(Guid messageId);

        Task<PublishResult> Publish(string topic, MessageCrateReqDto message);
    }
}
