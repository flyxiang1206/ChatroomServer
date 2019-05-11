using System;
using System.Collections.Generic;
using System.Text;

namespace ChatroomServer
{
    public interface IBasicData<T> where T : class
    {
        byte[] ToBytes();

        T FromBytes(byte[] bytes);
    }
}
