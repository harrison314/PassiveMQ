using Harrison314.PassiveMQ.Client.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Harrison314.PassiveMQ.Client
{
    public class Queu
    {
        public Guid Id
        {
            get;
            set;
        }

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

        public static Queu FromDto(QueuDto dto)
        {
            if (dto == null) return null;

            return new Queu()
            {
                Created = dto.Created.Value,
                Id = dto.Id.Value,
                Name = dto.Name,
                NotificationAdress = dto.NotificationAdress,
                TopicPattern = dto.TopicPattern
            };
        }
    }
}
