using System;
using System.Collections.Generic;
using System.Text;
using Lidgren.Network;

namespace ChatroomServer
{
    public class Package
    {
        public uint protocol { get; set; }

        public byte[] data { get; set; }

        public NetConnection user { get; set; }

        public Package() { }

        #region 多載
        public Package(uint protocol)
        {
            this.protocol = protocol;
        }
        public Package(uint protocol, byte[] data)
        {
            this.protocol = protocol;
            this.data = data;
        }
        public Package(uint protocol, byte[] data, NetConnection user)
        {
            this.protocol = protocol;
            this.data = data;
            this.user = user;
        }
        public Package(uint protocol, NetConnection user)
        {
            this.protocol = protocol;
            this.user = user;
        }
        #endregion
    }
}
