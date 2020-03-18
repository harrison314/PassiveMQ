using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Models.Exceptions
{
    public class PassiveMQNotFoundException : PassiveMQException
    {
        public PassiveMQNotFoundException()
        {
        }

        public PassiveMQNotFoundException(string entityName, Guid id)
            : base($"{entityName} with id {id} not found.")
        {
        }

        public PassiveMQNotFoundException(string message) 
            : base(message)
        {
        }

        public PassiveMQNotFoundException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }

        protected PassiveMQNotFoundException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}
