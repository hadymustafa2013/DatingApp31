﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingApp31.Dtos
{
    public class MessageForCreationDto
    {
        public int SenderId { get; set; }
        public int RecepientId { get; set; }
        public string Content { get; set; }
        public DateTime MessageSent { get; set; }

        public MessageForCreationDto()
        {
            MessageSent = DateTime.Now;
        }
    }
}
