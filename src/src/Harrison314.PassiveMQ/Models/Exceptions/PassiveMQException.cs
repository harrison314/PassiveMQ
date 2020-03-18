using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Models.Exceptions
{
    public class PassiveMQException : ApplicationException
    {
        public PassiveMQException()
        {
        }

        public PassiveMQException(string message) 
            : base(message)
        {
        }

        public PassiveMQException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }

        protected PassiveMQException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}
