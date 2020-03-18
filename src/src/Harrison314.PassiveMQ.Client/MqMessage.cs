using Harrison314.PassiveMQ.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Client
{
    public class MqMessage
    {
        public Guid Id
        {
            get;
            set;
        }
        public int? RetryCount { get; private set; }
        public DateTime? InsertionTime { get; private set; }
        public string Label { get; private set; }
        public DateTime? NextVisibleTime { get; private set; }
        public string Content { get; private set; }

        public string AsString
        {
            get => this.Content;
        }

        public byte[] AsByteArray
        {
            get => Convert.FromBase64String(this.Content);
        }

        public MqMessage()
        {

        }

        internal static MqMessage FromDto(MessageDto dto)
        {
            if (dto == null) return null;

            MqMessage message = new MqMessage()
            {
                Content = dto.Content,
                Id = dto.Id.Value,
                InsertionTime = dto.InsertionTime,
                Label = dto.Label,
                NextVisibleTime = dto.NextVisibleTime,
                RetryCount = dto.RetryCount
            };

            return message;
        }

        internal MessageDto ToDto()
        {
            return new MessageDto()
            {
                Content = this.Content,
                Id = this.Id,
                InsertionTime = this.InsertionTime,
                Label = this.Label,
                NextVisibleTime = this.NextVisibleTime,
                RetryCount = this.RetryCount
            };
        }
    }
}
