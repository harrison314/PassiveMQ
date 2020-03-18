using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Models
{
    public class PublishResult
    {
        public List<PublishPair> CratedMessages
        {
            get;
            set;
        }

        public PublishResult()
        {
            this.CratedMessages = new List<PublishPair>();
        }
    }
}
