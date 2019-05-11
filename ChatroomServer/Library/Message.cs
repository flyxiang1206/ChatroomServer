using System;
using System.Collections.Generic;
using System.Text;

namespace ChatroomServer
{
    public class Message
    {
        public User user { get; set; }
        public string message { get; set; }

        public Message(User user, string message)
        {
            this.user = user;
            this.message = message;
        }
    }
}
