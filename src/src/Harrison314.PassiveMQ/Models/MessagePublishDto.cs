using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Models
{
    public class MessagePublishDto
    {

        [Required]
        [RegularExpression("(/[A-Za-z0-9_]+)+")]
        public string Topic
        {
            get;
            set;
        }

        [Required]
        public MessageCrateReqDto Message
        {
            get;
            set;
        }

        public MessagePublishDto()
        {

        }
    }
}
