using System;
using System.Timers;
using System.Collections.Generic;
using Lidgren.Network;

namespace ChatroomServer
{
    class ServerMain
    {
        public static Timer TimerPackage = new Timer()
        {
            AutoReset = true,
            Enabled = true,
            Interval = 10
        };
        public static Timer TimerSendToClient = new Timer()
        {
            AutoReset = true,
            Enabled = true,
            Interval = 10
        };
        
        public static MyQueue<Message> oldMessage = new MyQueue<Message>(10);
        public static List<NetConnection> allConnections = new List<NetConnection>();
        public static Dictionary<uint, User> allUser = new Dictionary<uint, User>();

        public static Queue<Package> toClient = new Queue<Package>();
        public static Queue<Package> toAllClient = new Queue<Package>();

        public static uint uIdMax { get; set; }

        public static Server server = new Server();

        static void Main(string[] args)
        {
            uIdMax = 0;
            server.ServerStart();

            TimerPackage.Elapsed += new ElapsedEventHandler(server.ServerHandleMessage);
            TimerPackage.Elapsed += new ElapsedEventHandler(server.ClientConnection);
            TimerPackage.Start();

            TimerSendToClient.Elapsed += new ElapsedEventHandler(server.SendToClient);
            TimerSendToClient.Elapsed += new ElapsedEventHandler(server.SendToAllClient);
            TimerSendToClient.Start();

            while (true) ;
        }
    }
}
