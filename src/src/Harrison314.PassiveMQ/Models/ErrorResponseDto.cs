using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Models
{
    public class ErrorResponseDto
    {
        public string ExceptionName
        {
            get;
            set;
        }

        public string[] Messages
        {
            get;
            set;
        }

        public string DeveloperMeesage
        {
            get;
            set;
        }

        public ErrorResponseDto()
        {

        }
    }
}
