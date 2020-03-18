using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Client
{
    public class PassiveMqException : ApplicationException
    {
        public PassiveMqException()
        {
        }

        public PassiveMqException(Models.ErrorResponseDto errorResponseDto)
            :base(string.Join("; ", errorResponseDto.Messages))
        {
        }

        public PassiveMqException(string message) : base(message)
        {
        }

        public PassiveMqException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected PassiveMqException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
