using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChatLib;
using System.Net;
using System.Net.Sockets;
using System.Globalization;

namespace ServerTest
{
    public class ServerService : IServerService
    {
        public void OnAccept(ConnectionState state)
        {
            try
            {
                IPEndPoint endPoint = state.Connection.RemoteEndPoint as IPEndPoint;
                string message = string.Format("{0} has connected", endPoint);
                Log(message);
            }
            catch (Exception ex)
            {
                state.Error = ex;
                this.OnError(state);
            }
        }

        public void OnReceive(ConnectionState state)
        {
            if (state.Length > 0)
            {
                string log;
                int length = state.Length;
                byte[] buffer = state.Buffer;

                string message = TextEncoder.Decode(buffer, length);
                log = message;
                if (message.StartsWith("JOIN ")) // User Login
                {
                    // Replace the code from the message
                    message = message.Replace("JOIN ", string.Empty);
                    string username = message;
                    // Add new user
                    User user = new User() { Connection = state.Connection, Name = username };
                    Program.ServerManager.Clients.Add(user);
                    // Re-encode with the "has joined"
                    message += " has joined";
                    // Sent new values
                    buffer = TextEncoder.Encode(message);
                    length = buffer.Length;
                    log = string.Format("{0} <{1}> has joined", username, state.Connection.RemoteEndPoint);
                    // Send current user list (TODO)
                    //SendUserList(state.Connection);
                }
                Program.ServerManager.Broadcast(buffer, length); // Broadcast it!
                Log(log);
            }
        }

        private void SendUserList(Socket client)
        {
            const string command = "LIST ";
            foreach (var user in Program.ServerManager.Clients)
            {
                string message = command + user.Name;
                byte[] data = TextEncoder.Encode(message);
                Program.ServerManager.Send(data, client);
            }
        }

        public void OnDisconnect(ConnectionState state)
        {
            try
            {
                // Broadcast that they have left the room
                // Get user
                User user = Program.ServerManager.FindUserByEndPoint(state.Connection.RemoteEndPoint);
                if (!state.UserRemoved) Program.ServerManager.RemoveClient(state.Connection.RemoteEndPoint);
                if (user == null) user = new User() { Name = "Unkown" };
                string message = string.Format("{0} has left", user.Name);
                byte[] text = TextEncoder.Encode(message);
                Program.ServerManager.Broadcast(text, text.Length);
                Log(string.Format("{0} <{1}> has left", user.Name, state.Connection.RemoteEndPoint));
            }
            catch (Exception ex)
            {
                this.OnError(new ConnectionState() { Error = ex });
            }
        }

        public void OnError(ConnectionState state)
        {
            if (state.Error is SocketException)
            {
                SocketException ex = (SocketException)state.Error;

                // handle the error or otherwise report it
                switch (ex.ErrorCode)
                {
                    case 10054: // Connection closed
                        this.OnDisconnect(state);
                        return;
                    default:
                        break;
                }

                Log(string.Format("Error [{0}] - {1}: {2}\n{3}", ex.ErrorCode, state.Connection.RemoteEndPoint, ex.Message, ex.StackTrace));
            }
            else
            {
                Log(string.Format("Error : {0}", state.Error.Message));
            }
        }

        public void Log(string log)
        {
            DateTime now = DateTime.Now;
            string date = now.ToString("[yyyy-MM-dd HH:mm:ss]", CultureInfo.InvariantCulture);
            Console.WriteLine(date + " " + log);
        }
    }
}
