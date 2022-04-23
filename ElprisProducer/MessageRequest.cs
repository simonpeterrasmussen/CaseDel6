using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElprisProducer
{
    //When a reuester wants something a request is send
    public class MessageRequest
    {
        public Guid MessageId { get; set; }      // Unique id of message.
        public string? ReplyTopic { get; set; }  // Where does the requester wants the reply to come?
        public string? RequestType { get; set; } // What is the request for?
        public DateTime CreationDateTime { get; set; }   // When was the message created.

        public MessageRequest()
        {
        }
    }
}
