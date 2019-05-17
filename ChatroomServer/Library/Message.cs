using System;
using System.Collections.Generic;
using System.Text;

namespace ChatroomServer
{
    public class Message
    {
        public User user { get; set; }
        public string message { get; set; }
        public int hour { get; set; }
        public int min { get; set; }

        public Message()
        {
            user = new User();
            message = "";
            hour = 0;
            min = 0;
        }
        public Message(User user, string message)
        {
            this.user = user;
            this.message = message;
        }
    }
}
