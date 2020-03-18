using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Services.Implementation
{
    public class WebHookMessage
    {
        public Guid QueuId
        {
            get;
            set;
        }

        public Guid MessageId
        {
            get;
            set;
        }

        public DateTime Time
        {
            get;
            set;
        }

        public WebHookMessage()
        {

        }
    }
}
