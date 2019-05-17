using System;
using System.Collections.Generic;
using Lidgren.Network;

namespace ChatroomServer
{
    public class User
    {
        public uint uid { get; set; }
        public string name { get; set; }
        public NetConnection connection { get; set; }

        public User()
        {
            uid = 0;
            name = "";
            connection = null;
        }
        public User(uint uid, string name, NetConnection connection)
        {
            this.uid = uid;
            this.name = name;
            this.connection = connection;
        }
    }
}
