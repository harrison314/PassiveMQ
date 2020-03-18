using Harrison314.PassiveMQ.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Services.Implementation
{
    public class TimeAccessor : ITimeAccessor
    {
        public DateTime Now
        {
            get => DateTime.Now;
        }

        public DateTime UtcNow
        {
            get => DateTime.UtcNow;
        }

        public TimeAccessor()
        {

        }
    }
}
