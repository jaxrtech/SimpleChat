using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatLib
{
    public class ClientService : IClientService
    {
        /* 
         * TODO: Write chat server service implimentation from the client and server
         * Make sure to have it so that the messages get passed down throgh a notification interface
         * 
         */ 

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
