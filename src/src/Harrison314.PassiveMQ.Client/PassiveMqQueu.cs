using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Client
{
    public class PassiveMqQueu
    {
        public Guid Id
        {
            get;
            protected internal set;
        }

        public string Name
        {
            get;
            protected internal set;
        }

        public string TopicPattern
        {
            get;
            protected internal set;
        }

        public string NotificationAdress
        {
            get;
            protected internal set;
        }

        public DateTime Created
        {
            get;
            protected internal set;
        }

        internal PassiveMqQueu()
        {

        }

        public PassiveMqQueu(string name)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Parameter name is empty or whitespace.");

            this.Name = name;
            this.TopicPattern = null;
        }

        public PassiveMqQueu(string name, string topicPattern)
        {
            if (name == null) throw new ArgumentNullException(nameof(name));
            if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Parameter name is empty or whitespace.");

            this.Name = name;
            this.TopicPattern = topicPattern;
        }
    }
}
