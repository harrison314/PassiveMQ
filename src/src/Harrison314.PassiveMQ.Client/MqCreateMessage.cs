using Harrison314.PassiveMQ.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Client
{
    public class MqCreateMessage
    {
        public string Label { get; internal set; }

        public string Content { get; internal set; }

        internal MqCreateMessage()
        {

        }

        public MqCreateMessage(string content, string label = null)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));

            this.Content = content;
            this.Label = label;
        }

        public MqCreateMessage(byte[] content, string label = null)
        {
            if (content == null) throw new ArgumentNullException(nameof(content));

            this.Content = Convert.ToBase64String(content);
            this.Label = label;
        }

        internal MessagePublishDtoMessage ToDto()
        {
            return new MessagePublishDtoMessage()
            {
                Content = this.Content,
                Label = this.Label
            };
        }
    }
}
