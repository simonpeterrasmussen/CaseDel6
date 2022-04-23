﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElprisRequester
{
    public class MessageReply
    {
        public Guid MessageId { get; set; }     // Unique id of message.
        public Guid ReplyOnRequestID { get; set; }  // Unique id of message.
        public DateTime CreationDateTime { get; set; }   // When was the message created.
        public string Replydata { get; set; }       // The data reply to the requester.

        public MessageReply()
        {
        }
    }
}
