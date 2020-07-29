using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace PC_Controller
{
    public static class Controller
    {
        public static Socket socket;

        public static void Connect()
        {
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse("10.0.0.160"), 255);
            socket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ipe);
            Communication.Main.socket = socket;
        }
    }
}
