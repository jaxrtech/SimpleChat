using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChatLib;
using System.Net;
using System.Net.Sockets;
using System.Diagnostics;

namespace ClientTest
{
    class ClientService : IClientService
    {
        public void OnConnect(ConnectionState state)
        {
            Debug.WriteLine("Client - OnConnect");
            IPEndPoint endPoint = state.Connection.RemoteEndPoint as IPEndPoint;
            string message = string.Format("Connected to server on {0}:{1}", endPoint.Address, endPoint.Port);
            Console.WriteLine(message);
            byte[] data = TextEncoder.Encode("Hello world");
            state.Connection.Send(data, 0, data.Length, SocketFlags.None);
            Console.WriteLine("Sent message");
        }

        public void OnReceive(ConnectionState state)
        {
            Debug.WriteLine("Client - OnReceive");
            string message = TextEncoder.Decode(state.Buffer, state.Length);
            Console.WriteLine("Server: {0}", message);
        }

        public void OnDisconnect(ConnectionState state)
        {
            Debug.WriteLine("Client - OnDisconnect");
            Console.WriteLine("Disconnected from server");
        }

        public void OnError(ConnectionState state)
        {
            Debug.WriteLine("Client - OnError");
            Console.WriteLine("Error: {0}", state.Error.Message);
        }
    }
}
