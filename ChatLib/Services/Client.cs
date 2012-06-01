using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;

namespace ChatLib
{
    public class Client
    {
        /// <summary>
        /// The service that will handle server data
        /// </summary>
        private IClientService Service;

        /// <summary>
        /// Socket that is talking with connection
        /// </summary>
        Socket Connection;

        /// <summary>
        /// If the client is currently connected to the server
        /// </summary>
        public bool IsConnected { get { return Connection.Connected; } }

        public Client(IClientService service)
        {
            Service = service;
        }

        /// <summary>
        /// Start the server by listening for clients
        /// </summary>
        /// <param name="address">The IP address or host address to connect to</param>
        /// <param name="port">The port to connect on</param>
        /// <returns>If the address is valid and the server was able to start listening</returns>
        public bool Start(string address, int port)
        {
            IPAddress ip;
            bool valid = Address.TryParseAddress(address, out ip);
            if (valid) return Start(ip, port);
            else return false;
        }

        /// <summary>
        /// Start the server by listening for clients
        /// </summary>
        /// <param name="address">The IP address to connect to</param>
        /// <param name="port">The port to connect on</param>
        /// <returns>If the server was able to start listening</returns>
        public bool Start(IPAddress address, int port)
        {
            try
            {
                // Init connection socket
                Connection = new Socket(AddressFamily.InterNetwork,
                                      SocketType.Stream,
                                      ProtocolType.Tcp);

                // Set the connection to the server
                IPEndPoint server = new IPEndPoint(address, port);

                // Connect to the server
                Connection.BeginConnect(server, new AsyncCallback(OnConnect), null);
                return true;
            }
            catch (Exception ex)
            {
                Service.OnError(new ConnectionState() { Error = ex });
                return false;
            }
        }

        /// <summary>
        /// Disconnects from the server
        /// </summary>
        /// <returns></returns>
        public bool Disconnect()
        {
            ConnectionState state = new ConnectionState();

            try
            {
                Connection.Shutdown(SocketShutdown.Both);
                //state.Connection.BeginDisconnect(true, new AsyncCallback(OnDisconnect), state);
                Connection.Close();

                state.Connection = Connection;
                Service.OnDisconnect(state);
                return true;
            }
            catch (Exception ex)
            {
                state.Error = ex;
                Service.OnError(state);
                return false;
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
            }
        }

        /// <summary>
        /// Sends a message to the server
        /// </summary>
        /// <param name="data">The data to send to the server</param>
        /// <returns>If the client was able to send the message</returns>
        public bool Send(byte[] data)
        {
            ConnectionState state = new ConnectionState();
            state.Connection = Connection;
            state.Length = data.Length;
            state.Buffer = data;

            try
            {
                Connection.BeginSend(state.Buffer, 0, state.Length, SocketFlags.None, new AsyncCallback(OnSend), state);
                return true;
            }
            catch (Exception ex)
            {
                state.Error = ex;
                Service.OnError(state);
                return false;
            }
        }

        private void OnConnect(IAsyncResult ar)
        {
            ConnectionState state = new ConnectionState();
            state.Connection = Connection;

            try
            {
                state.Connection.EndConnect(ar);
                state.Connection.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None,
                    new AsyncCallback(OnReceive), state);
                Service.OnConnect(state);
            }
            catch (Exception ex)
            {
                state.Error = ex;
                Service.OnError(state);
            }
        }

        private void OnReceive(IAsyncResult ar)
        {
            ConnectionState state = (ConnectionState)ar.AsyncState;

            try
            {
                state.Length = state.Connection.EndReceive(ar); // message length
                state.Connection.BeginReceive(state.Buffer, 0, state.Buffer.Length, SocketFlags.None,
                    new AsyncCallback(OnReceive), state);
                Service.OnReceive(state);
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
            }
            catch (Exception ex)
            {
                state.Error = ex;
                Service.OnError(state);
            }
        }
    }
}