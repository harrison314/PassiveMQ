using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Models
{
    public class QueuCreateReqDto
    {

        [Required]
        public string Name
        {
            get;
            set;
        }

        [RegularExpression("(/[A-Za-z0-9_]+)+")]
        public string TopicPattern
        {
            get;
            set;
        }

        public string NotificationAdress
        {
            get;
            set;
        }

        public QueuCreateReqDto()
        {

        }
    }
}
