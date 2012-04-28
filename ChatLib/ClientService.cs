using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatLib
{
    public class ClientService : IClientService
    {
        public void OnConnect(ConnectionState state)
        {
            Console.WriteLine("Connected to server on {0}", state.Connection.RemoteEndPoint);
        }

        public void OnReceive(ConnectionState state)
        {
            string message = TextEncoder.Decode(state.Buffer, state.Length);
            Console.WriteLine("Server: {0}", message);
        }

        public void OnDisconnect(ConnectionState state)
        {
            Console.WriteLine("Disconnected from server");
        }

        public void OnError(ConnectionState state)
        { }
    }
}
