using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
using System.Diagnostics;

namespace ChatLib
{
    // TODO: Need to add an exception manager so that it can properly know what to do
    //       when an exception is thrown, eg: remove the client from the client list
    public class Server
    {
        /// <summary>
        /// The service that will handle client data
        /// </summary>
        private IServerService Service;

        /// <summary>
        /// List of currently connected clients
        /// </summary>
        private List<User> _Clients = new List<User>();
        public List<User> Clients
        {
            get { return _Clients; }
            private set { _Clients = value; }
        }

        public Server(IServerService service)
        {
            Service = service;
        }

        /// <summary>
        /// Start the server by listening for clients
        /// </summary>
        /// <returns>If the server was able to start listening</returns>
        public bool Start(int port)
        {
            try
            {
                // Init listener socket
                Socket listener = new Socket(AddressFamily.InterNetwork,
                                      SocketType.Stream,
                                      ProtocolType.Tcp);

                // Assign any IP of this local machine on the port number
                IPEndPoint local = new IPEndPoint(IPAddress.Any, port);

                // Bind and listen on the address
                listener.Bind(local);
                listener.Listen(100);

                // Accept connecting client
                listener.BeginAccept(new AsyncCallback(OnAccept), listener);
                return true;
            }
            catch (Exception ex)
            {
                Service.OnError(new ConnectionState() { Error = ex });
                return false;
                throw;
            }
        }

        /// <summary>
        /// Tries to remove the client that is located on the end point
        /// </summary>
        /// <param name="remoteEndPoint">The end point where the client is located</param>
        /// <returns>If the the client was found and removed</returns>
        public bool RemoveClient(EndPoint remoteEndPoint)
        {
            int count = 0;
            foreach (var client in Clients)
            {
                if (client.Connection.RemoteEndPoint == remoteEndPoint)
                {
                    this.Clients.RemoveAt(count);
                    return true;
                }
                count++;
            }
            // not found
            return false;
        }

        /// <summary>
        /// Tries to find the client that is located on the end point
        /// </summary>
        /// <param name="remoteEndPoint">The end point where the client is located</param>
        /// <returns>The index of the client on the list unless it was unfound and will return -1</returns>
        public int FindUserIndexByEndPoint(EndPoint remoteEndPoint)
        {
            int count = 0;
            foreach (var client in Clients)
            {
                if (client.Connection.RemoteEndPoint == remoteEndPoint)
                {
                    return count;
                }
            }
            // not found
            return -1;
        }

        /// <summary>
        /// Tries to find the client that is using the username
        /// </summary>
        /// <param name="username">The username to find</param>
        /// <returns>The user that was found or null if it was not found</returns>
        public User FindUserByName(string username)
        {
            foreach (var client in Clients)
            {
                if (client.Name == username)
                {
                    return client;
                }
            }
            // not found
            return null;
        }

        /// <summary>
        /// Tries to find the client that is using the EndPoint
        /// </summary>
        /// <param name="remoteEndPoint">The EndPoint where the client it located</param>
        /// <returns>The user that was found or null if it was not found</returns>
        public User FindUserByEndPoint(EndPoint remoteEndPoint)
        {
            foreach (var client in Clients)
            {
                if (client.Connection.RemoteEndPoint == remoteEndPoint)
                {
                    return client;
                }
            }
            // not found
            return null;
        }

        /// <summary>
        /// Sends data to a client
        /// </summary>
        /// <param name="data">The data to send to the client</param>
        /// <param name="client">The client to send it to</param>
        /// <returns>If the server was able to send the message</returns>
        public bool Send(byte[] data, Socket client)
        {
            return Send(data, data.Length, client);
        }

        /// <summary>
        /// Sends data to a client
        /// </summary>
        /// <param name="data">The buffer to send to the client</param>
        /// <param name="length">The length of the buffer</param>
        /// <param name="client">The client to send it to</param>
        /// <returns>If the server was able to send the message</returns>
        public bool Send(byte[] data, int length, Socket client)
        {
            ConnectionState state = new ConnectionState();
            state.Connection = client;

            // Make sure that the client is connected or else remove his
            if (!client.Connected)
            {
                state.UserRemoved = true;
                RemoveClient(client.RemoteEndPoint);
                Service.OnDisconnect(state);
            }

            state.Buffer = data;
            state.Length = length;

            try
            {
                state.Connection.BeginSend(state.Buffer, 0, state.Length, SocketFlags.None, new AsyncCallback(OnSend), state);
                return true;
            }
            catch (Exception ex)
            {
                state.Error = ex;
                Service.OnError(state);
                return false;
            }
        }

        /// <summary>
        /// Sends data to all clients on the list
        /// </summary>
        /// <param name="data"></param>
        public void Broadcast(byte[] data, int length)
        {
            foreach (var client in Clients)
            {
                Send(data, length, client.Connection);
            }
        }

        /// <summary>
        /// Disconnects a client from the server
        /// </summary>
        /// <param name="client">The client socket to disconnect</param>
        /// <returns>If the server was able to send the message</returns>
        public bool Disconnect(Socket client)
        {
            ConnectionState state = new ConnectionState();

            try
            {
                client.Shutdown(SocketShutdown.Both);
                //state.Connection.BeginDisconnect(true, new AsyncCallback(OnDisconnect), state);
                client.Close();
                state.Connection = client;
                Service.OnDisconnect(state);
                return true;
            }
            catch (Exception ex)
            {
                state.Error = ex;
                Service.OnError(state);
                return false;

                throw;
            }
        }

        private void OnDisconnect(IAsyncResult ar)
        {
            ConnectionState state = (ConnectionState)ar.AsyncState;

            try
            {
                state.Connection.EndDisconnect(ar);
                Service.OnDisconnect(state);
            }
            catch (Exception ex)
            {
                state.Error = ex;
                Service.OnError(state);

                throw;
            }
        }

        private void OnAccept(IAsyncResult ar)
        {
            ConnectionState state = new ConnectionState();

            try
            {
                Socket listener = (Socket)ar.AsyncState;
                Socket client = listener.EndAccept(ar);

                state.Connection = client;

                // Start listening for data the client sends
                client.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None,
                    new AsyncCallback(OnReceive), state);

                // Call the service
                Service.OnAccept(state);

                // Resume listening for more clients
                listener.BeginAccept(new AsyncCallback(OnAccept), listener);
            }
            catch (Exception ex)
            {
                state.Error = ex;
                Service.OnError(state);
                throw;
            }
        }

        private void OnReceive(IAsyncResult ar)
        {
            ConnectionState state = (ConnectionState)ar.AsyncState;
            
            try
            {
                int read = state.Connection.EndReceive(ar); // message length
                state.Length = read;

                // Only run this if there is data to be read
                if (read > 0)
                {
                    // TODO: Make sure to create a new Data object and set it so that it is not done reading data
                    //       unless the reading is done

                    // Make sure client is still connected
                    if (!state.Connection.Connected)
                    {
                        Service.OnDisconnect(state);
                    }
                    else
                    {
                        state.Connection.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None,
                            new AsyncCallback(OnReceive), state);
                        Debug.WriteLine("Recieved data from " + state.Connection.RemoteEndPoint, "Recieve");
                        Service.OnReceive(state);
                    }
                }
                else // user must of disconnected 
                {
                    Service.OnDisconnect(state);
                }
            }
            catch (Exception ex)
            {
                state.Error = ex;
                Service.OnError(state);
            }
        }

        private void OnSend(IAsyncResult ar)
        {
            ConnectionState state = (ConnectionState)ar.AsyncState;

            try
            {
                state.Connection.EndSend(ar);

                Debug.WriteLine("Data send to " + state.Connection.RemoteEndPoint, "Send");
            }
            catch (Exception ex)
            {
                state.Error = ex;
                Service.OnError(state);
            }
        }
    }
}
