using System;
using System.Collections.Generic;
using System.Text;

namespace ChatroomServer
{
    public enum Protocol
    {
        LOGIN = 0x00000000,
        SEND = 0x00001000,
        SECRET = 0x0000F000,
        ADDUSER = 0x00000011,
        KILLUSER = 0x000000FF
    }
}
