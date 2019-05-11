using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lidgren.Network;
using System.Threading;

namespace ChatroomServer
{
    class Server
    {
        public NetServer server;
        public NetPeerConfiguration config = new NetPeerConfiguration("game_server");
        public NetConnection client;

        #region 開啟Server
        public void ServerStart()
        {
            try
            {
                config.Port = 14200;
                config.MaximumConnections = 100;
                config.PingInterval = 1f;
                config.ConnectionTimeout = 10f;
                server = new NetServer(config);

                SynchronizationContext.SetSynchronizationContext(new SynchronizationContext());
                //server.RegisterReceivedCallback(new SendOrPostCallback(ServerHandleMessage));

                server.Start();
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "]" + "Server Start\n");
            }
            catch (Exception e)
            {
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "]" + e.ToString());
            }
        }
        #endregion

        #region 讓Client連接
        public void ClientConnection(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                ServerMain.TimerPackage.Stop();
                NetIncomingMessage incMsg = server.ReadMessage();
                if (incMsg == null)
                    return;

                //Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "]" + "ClientConnection");

                //啟用連接批准
                config.EnableMessageType(NetIncomingMessageType.ConnectionApproval);
                if (incMsg != null)
                {
                    switch (incMsg.MessageType)
                    {
                        case NetIncomingMessageType.ConnectionApproval:
                            string s = incMsg.ReadString();
                            if (s == "i_want_chat")
                                incMsg.SenderConnection.Approve();
                            else
                                incMsg.SenderConnection.Deny();
                            break;

                        default:

                            break;
                    }
                }
            }
            catch (Exception er)
            {
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "]" + er.ToString());
            }
            finally
            {
                ServerMain.TimerPackage.Start();
            }
        }
        #endregion

        #region 讓data,ServerHandle
        public void ServerHandleMessage(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                #region HandleMessage
                ServerMain.TimerPackage.Stop();
                NetIncomingMessage im;
                string message = string.Empty;
                Byte[] Bymessage;
                //ServerHandler handler;

                if ((im = server.ReadMessage()) == null)
                    return;

                //Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "]" + "ServerHandleMessage");
                client = im.SenderConnection;
                // handle incoming message
                switch (im.MessageType)
                {
                    case NetIncomingMessageType.DebugMessage:
                    case NetIncomingMessageType.ErrorMessage:
                    case NetIncomingMessageType.WarningMessage:
                    case NetIncomingMessageType.VerboseDebugMessage:
                        message = im.ReadString();
                        Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "]" + message);
                        break;

                    case NetIncomingMessageType.StatusChanged:
                        NetConnectionStatus status = (NetConnectionStatus)im.ReadByte();
                        Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "]" + status.ToString());

                        if (status == NetConnectionStatus.Connected)
                        {
                            Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "]" + NetUtility.ToHexString(im.SenderConnection.RemoteUniqueIdentifier));
                            //Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "]" + "Remote hail: " + im.SenderConnection.RemoteHailMessage.ReadString() + " from " + im.SenderEndPoint.ToString());
                            client = im.SenderConnection;
                            Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "]" + im.SenderConnection.ToString());
                            Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "]" + "Ping = " + client.AverageRoundtripTime.ToString() + " S");
                        }
                        if (status == NetConnectionStatus.Disconnected)
                        {

                        }
                        Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "]" + "Peer:" + im.SenderConnection.Peer.ToString());

                        break;
                    case NetIncomingMessageType.Data:
                        Bymessage = im.ReadBytes(4);
                        Package package = new Package(BitConverter.ToUInt32(Bymessage, 0));
                        //Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "]" + "[C-->S] " + (Protocol)package.protocol + "\n");
                        int _length = BitConverter.ToInt32(im.ReadBytes(4), 0);
                        List<byte> _data = new List<byte>();
                        for (int i = 0; i < _length; i++)
                            _data.AddRange(im.ReadBytes(1));
                        package.data = _data.ToArray();
                        package.user = client;

                        switch (package.protocol)
                        {
                            case (uint)Protocol.LOGIN:
                                inLogin(package);
                                break;
                            case (uint)Protocol.SEND:
                                inSend(package);
                                break;
                            case (uint)Protocol.SECRET:
                                isSecret(package);
                                break;
                        }
                        break;
                    default:
                        Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "]" + "Unhandled type: " + im.MessageType + " " + im.LengthBytes + " bytes " + im.DeliveryMethod + "|" + im.SequenceChannel);
                        break;
                }
                server.Recycle(im);
                #endregion
            }
            catch (Exception er)
            {
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "]" + er.ToString());
            }
            finally
            {
                ServerMain.TimerPackage.Start();
            }
        }
        #endregion

        #region 給Clinet訊息
        public void SendToAllClient(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                ServerMain.TimerSendToClient.Stop();
                if (ServerMain.toAllClient.Count == 0) return;

                Package data = (Package)ServerMain.toAllClient.Dequeue();
                List<byte> da = new List<byte>();
                da.AddRange(BitConverter.GetBytes(data.protocol));
                if (data.data != null)
                {
                    da.AddRange(BitConverter.GetBytes(data.data.Length));
                    da.AddRange(data.data);
                }
                else
                {
                    da.AddRange(BitConverter.GetBytes(0));
                }

                NetOutgoingMessage om = server.CreateMessage();
                
                foreach (byte d in da)
                {
                    om.Write(d);
                }
                server.SendMessage(om,ServerMain.allConnections, NetDeliveryMethod.ReliableOrdered, 0);
                server.FlushSendQueue();
            }
            catch (Exception er)
            {
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "]" + er.ToString());
            }
            finally
            {
                ServerMain.TimerSendToClient.Start();
            }
        }
        public void SendToClient(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                ServerMain.TimerSendToClient.Stop();
                if (ServerMain.toClient.Count == 0) return;

                Package data = (Package)ServerMain.toClient.Dequeue();
                List<byte> da = new List<byte>();
                da.AddRange(BitConverter.GetBytes(data.protocol));
                if (data.data != null)
                {
                    da.AddRange(BitConverter.GetBytes(data.data.Length));
                    da.AddRange(data.data);
                }
                else
                {
                    da.AddRange(BitConverter.GetBytes(0));
                }
                
                NetOutgoingMessage om = server.CreateMessage();
                foreach (byte d in da)
                {
                    om.Write(d);
                }
                server.SendMessage(om, data.user, NetDeliveryMethod.ReliableOrdered, 0);
                server.FlushSendQueue();
            }
            catch (Exception er)
            {
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "]" + er.ToString());
            }
            finally
            {
                ServerMain.TimerSendToClient.Start();
            }
        }
        #endregion

        private void inLogin(Package package)
        {
            try
            {
                ServerMain.uIdMax++;
                clsLogin data = new clsLogin(0,"");
                data = data.FromBytes(package.data);
                User user = new User(ServerMain.uIdMax, data.userName,package.user);

                ServerMain.allConnections.Add(package.user);
                ServerMain.allUser.Add(user.uid,user);

                data.uid = user.uid;
                package.data = data.ToBytes();
                ServerMain.toClient.Enqueue(package);
            }
            catch (Exception e)
            {
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "]" + e.ToString());
            }
        }

        private void inSend(Package package)
        {
            try
            {
                ServerMain.toAllClient.Enqueue(package);
            }
            catch (Exception e)
            {
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "]" + e.ToString());
            }
        }

        private void isSecret(Package package)
        {
            try
            {
                ServerMain.toClient.Enqueue(package);
            }
            catch (Exception e)
            {
                Console.WriteLine("[" + DateTime.Now.ToString("HH:mm:ss") + "]" + e.ToString());
            }
        }
    }
}
