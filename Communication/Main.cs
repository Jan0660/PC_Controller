using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Net;
using System.Net.Sockets;
using System.Text;
namespace Communication
{
    public static class Main
    {
        public static Socket socket;

        public static void Connect(string ipString)
        {
            IPEndPoint ipe = new IPEndPoint(IPAddress.Parse(ipString), 255);
            socket = new Socket(ipe.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(ipe);
        }

        public static string SendAndReceive(string send)
        {
            Byte[] bytesSent = Encoding.ASCII.GetBytes(send);
            // pussy out if socket is non-existent
            if (socket == null)
                return Codes.Error.ToString();
            socket.Send(bytesSent, bytesSent.Length, 0);
            return ReceiveData();
        }

        public static void SendData(string str)
        {
            byte[] bytesSent = Encoding.ASCII.GetBytes(str);

            if (socket == null)
            {
                // TODO: deal with it
            }
            Console.WriteLine("sent " + str);
            socket.Send(bytesSent, bytesSent.Length, 0);
        }

        public static string ReceiveData()
        {
            byte[] bytes = ReceiveBytes();
            string received = Encoding.ASCII.GetString(bytes, 0, bytes.Length);
            return received;
        }

        public static byte[] ReceiveBytes(int bufferSize = 256)
        {

            List<byte> received = new List<byte>();
            int bytes = 0;
            // receive bytes until there are no bytes to be received
            do
            {
                int available = socket.Available;
                if (available > 255)
                {
                    available = 255;
                }
                else if (available == 0)
                {
                    // just set the available to 1, because if there was a while loop to wait until there are
                    // bytes available the server app' CPU usage would be worse than yandere simulator's
                    // after the 1 byte has been received it will receive the rest  
                    available = 1;
                }
                byte[] bytesReceived = new byte[available];
                bytes = socket.Receive(bytesReceived, available, 0);
                received.AddRange(bytesReceived);
            }
            while (socket.Available != 0);
            return received.ToArray();
        }

        public static string Request(string str)
        {
            return SendAndReceive(str);
        }

        /// <summary>
        /// Sends a boolean which can be read by Codes.FromBool(string).
        /// </summary>
        /// <param name="toSend">Boolean to send</param>
        public static void SendBool(bool toSend)
        {
            SendData(Codes.FromBool(toSend).ToString());
        }
    }
}
