using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Models
{
    public class CountMessageDto
    {
        public Guid QueuId
        {
            get;
            set;
        }

        public int Count
        {
            get;
            set;
        }

        public CountMessageDto()
        {

        }
    }
}
