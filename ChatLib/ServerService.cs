using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace ChatLib
{
    public class ServerService : IServerService
    {
        public void OnAccept(ConnectionState state)
        {
            Console.WriteLine("Client connected: {0}", state.Connection.RemoteEndPoint);
        }

        public void OnReceive(ConnectionState state)
        {
            string message = TextEncoder.Decode(state.Buffer, state.Length);
            Console.WriteLine("{0}: {1}", state.Connection.RemoteEndPoint, message);
        }

        public void OnDisconnect(ConnectionState state)
        {
            Console.WriteLine("Client disconnected: {0}", state.Connection.RemoteEndPoint);
        }

        public void OnError(ConnectionState state)
        {
            Console.WriteLine("Error: {0}", state.Error.Message);
        }
    }
}
