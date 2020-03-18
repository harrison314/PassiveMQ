using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Services.Contracts
{
    public interface ITimeAccessor
    {
        DateTime Now
        {
            get;
        }

        DateTime UtcNow
        {
            get;
        }
    }
}
