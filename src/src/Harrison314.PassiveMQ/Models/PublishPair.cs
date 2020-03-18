using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Models
{
    public class PublishPair
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

        public PublishPair()
        {

        }
    }
}
