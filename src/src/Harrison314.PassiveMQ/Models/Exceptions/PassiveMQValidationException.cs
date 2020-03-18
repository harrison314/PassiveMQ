using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Models.Exceptions
{
    public class PassiveMQValidationException : PassiveMQException
    {
        public PassiveMQValidationException()
        {
        }

        public PassiveMQValidationException(string message) : base(message)
        {
        }

        public PassiveMQValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PassiveMQValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
