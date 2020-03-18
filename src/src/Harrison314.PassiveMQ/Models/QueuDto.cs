using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Models
{
    public class QueuDto
    {
        public Guid Id
        {
            get;
            set;
        }

        [Required]
        public string Name
        {
            get;
            set;
        }

        public string NotificationAdress
        {
            get;
            set;
        }

        public string TopicPattern
        {
            get;
            set;
        }

        public DateTime Created
        {
            get;
            set;
        }

        public QueuDto()
        {

        }
    }
}
