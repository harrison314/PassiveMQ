using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Services.Configuration
{
    public class MqSettings
    {
        public TimeSpan DefaultRetry
        {
            get;
            set;
        }

        public int MaxDefaultRetryCount
        {
            get;
            set;
        }

        public MqSettings()
        {

        }
    }
}
