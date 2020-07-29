using System;
using System.Net.Sockets;
using System.Text;

namespace Communication
{
    public static class Main
    {
        public static Socket socket;

        public enum Codes { Error, Success,
            True, False,
        }

        public static string SendAndReceive(string send)
        {
            Byte[] bytesSent = Encoding.ASCII.GetBytes(send);
            Byte[] bytesReceived = new Byte[256];
            string received = "";

            // pussy out if socket is non-existent
            if (socket == null)
                return ("Connection failed");

            // Send data to the server.
            socket.Send(bytesSent, bytesSent.Length, 0);

            int bytes = 0;
            bytes = socket.Receive(bytesReceived, bytesReceived.Length, 0);
            received = Encoding.ASCII.GetString(bytesReceived, 0, bytes);

            return received;
        }

        public static void SendData(string str)
        {
            Byte[] bytesSent = Encoding.ASCII.GetBytes(str);

            if (socket == null)
            {
                // TODO: deal with it
            }

            // Send data to the server.
            socket.Send(bytesSent, bytesSent.Length, 0);
        }

        public static string ReceiveData()
        {
            Byte[] bytesReceived = new Byte[256];
            string received = "";
            int bytes = 0;
            do
            {
                bytes = socket.Receive(bytesReceived, bytesReceived.Length, 0);
                received = Encoding.ASCII.GetString(bytesReceived, 0, bytes);
            }
            while (bytes == 0);
            return received;
        }

        public static string Request(string str)
        {
            return SendAndReceive(str);
        }
    }
}
