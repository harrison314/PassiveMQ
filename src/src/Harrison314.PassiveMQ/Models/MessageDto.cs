using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Models
{
    public class MessageDto
    {
        public Guid Id
        {
            get;
            set;
        }

        public DateTime InsertionTime
        {
            get;
            set;
        }

        public DateTime? NextVisibleTime
        {
            get;
            set;
        }

        public string Label
        {
            get;
            set;
        }

        public string Content
        {
            get;
            set;
        }

        public int RetryCount
        {
            get;
            set;
        }

        public MessageDto()
        {

        }
    }
}
