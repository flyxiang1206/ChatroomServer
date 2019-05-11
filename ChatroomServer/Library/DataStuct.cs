using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatroomServer
{
    public class clsLogin : IBasicData<clsLogin>
    {
        public uint uid { get; set; }
        private int userNameRange { get; set; }
        public string userName { get; set; }

        public clsLogin(uint uid, string userName)
        {
            this.uid = uid;
            this.userName = userName;
            userNameRange = Encoding.UTF8.GetBytes(userName).Length;
        }

        public byte[] ToBytes()
        {
            List<byte> result = new List<byte>();
            try
            {
                result.AddRange(BitConverter.GetBytes(uid));
                result.AddRange(BitConverter.GetBytes(userNameRange));
                result.AddRange(Encoding.UTF8.GetBytes(userName));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return result.ToArray();
        }

        public clsLogin FromBytes(byte[] bytes)
        {
            clsLogin result = new clsLogin(0, "");

            try
            {
                if (bytes.Count() == 0)
                {
                    throw new Exception("Wrong Length");
                }
                List<byte> list = bytes.ToList();

                result.uid = BitConverter.ToUInt32(list.GetRange(0, 4).ToArray(), 0);
                result.userNameRange = BitConverter.ToInt32(list.GetRange(4, 4).ToArray(), 0);
                result.userName = Encoding.UTF8.GetString(list.GetRange(8, result.userNameRange).ToArray());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return result;
        }
    }

    public class clsSend : IBasicData<clsSend>
    {
        public Message message { get; set; }

        private int userNameRange { get; set; }
        private int messageRange { get; set; }

        public clsSend(Message message)
        {
            this.message = message;
            messageRange = Encoding.UTF8.GetBytes(message.message).Length;
            userNameRange = Encoding.UTF8.GetBytes(message.user.name).Length;
        }

        public byte[] ToBytes()
        {
            List<byte> result = new List<byte>();
            try
            {
                result.AddRange(BitConverter.GetBytes(message.user.uid));
                result.AddRange(BitConverter.GetBytes(userNameRange));
                result.AddRange(Encoding.UTF8.GetBytes(message.user.name));
                result.AddRange(BitConverter.GetBytes(messageRange));
                result.AddRange(Encoding.UTF8.GetBytes(message.message));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return result.ToArray();
        }

        public clsSend FromBytes(byte[] bytes)
        {
            clsSend result = new clsSend(this.message);

            try
            {
                if (bytes.Count() == 0)
                {
                    throw new Exception("Wrong Length");
                }
                List<byte> list = bytes.ToList();

                result.message.user.uid = BitConverter.ToUInt32(list.GetRange(0, 4).ToArray(), 0);
                result.userNameRange = BitConverter.ToInt32(list.GetRange(4, 4).ToArray(), 0);
                result.message.user.name = Encoding.UTF8.GetString(list.GetRange(8, result.userNameRange).ToArray());
                result.messageRange = BitConverter.ToInt32(list.GetRange(8 + result.userNameRange, 4).ToArray(), 0);
                result.message.message = Encoding.UTF8.GetString(list.GetRange(12 + result.userNameRange, result.messageRange).ToArray());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return result;
        }
    }

    public class clsSecret : IBasicData<clsSecret>
    {
        public Message message { get; set; }
        public uint forUid { get; set; }

        private int userNameRange { get; set; }
        private int messageRange { get; set; }

        public clsSecret(Message message, uint forUid)
        {
            this.message = message;
            messageRange = Encoding.UTF8.GetBytes(message.message).Length;
            userNameRange = Encoding.UTF8.GetBytes(message.user.name).Length;
            this.forUid = forUid;
        }

        public byte[] ToBytes()
        {
            List<byte> result = new List<byte>();
            try
            {
                result.AddRange(BitConverter.GetBytes(message.user.uid));
                result.AddRange(BitConverter.GetBytes(forUid));
                result.AddRange(BitConverter.GetBytes(userNameRange));
                result.AddRange(Encoding.UTF8.GetBytes(message.user.name));
                result.AddRange(BitConverter.GetBytes(messageRange));
                result.AddRange(Encoding.UTF8.GetBytes(message.message));
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            return result.ToArray();
        }

        public clsSecret FromBytes(byte[] bytes)
        {
            clsSecret result = new clsSecret(this.message,0);

            try
            {
                if (bytes.Count() == 0)
                {
                    throw new Exception("Wrong Length");
                }
                List<byte> list = bytes.ToList();

                result.message.user.uid = BitConverter.ToUInt32(list.GetRange(0, 4).ToArray(), 0);
                result.forUid = BitConverter.ToUInt32(list.GetRange(4, 4).ToArray(), 0);
                result.userNameRange = BitConverter.ToInt32(list.GetRange(8, 4).ToArray(), 0);
                result.message.user.name = Encoding.UTF8.GetString(list.GetRange(12, result.userNameRange).ToArray());
                result.messageRange = BitConverter.ToInt32(list.GetRange(12 + result.userNameRange, 4).ToArray(), 0);
                result.message.message = Encoding.UTF8.GetString(list.GetRange(16 + result.userNameRange, result.messageRange).ToArray());
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            return result;
        }
    }
}
