using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Models
{
    public class MessageCrateReqDto
    {
        public string Label
        {
            get;
            set;
        }

        [Required]
        public string Content
        {
            get;
            set;
        }

        public MessageCrateReqDto()
        {

        }
    }
}
