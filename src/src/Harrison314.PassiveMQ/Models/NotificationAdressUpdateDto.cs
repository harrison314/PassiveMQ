using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Models
{
    public class NotificationAdressUpdateDto
    {
        public Guid QueuId
        {
            get;
            set;
        }

        public string NotificationAdress
        {
            get;
            set;
        }

        public NotificationAdressUpdateDto()
        {

        }
    }
}
