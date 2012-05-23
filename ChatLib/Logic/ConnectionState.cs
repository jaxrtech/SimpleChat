using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace ChatLib
{
    public class ConnectionState
    {
        /// <summary>
        /// The socket of the connection 
        /// </summary>
        public Socket Connection { get; set; }

        private bool _UserRemoved = false;
        /// <summary>
        /// If the user had been removed from the client list
        /// </summary>
        public bool UserRemoved
        {
            get { return _UserRemoved; }
            set { _UserRemoved = value; }
        }

        /// <summary>
        /// The size of the buffer
        /// </summary>
        public const int BufferSize = 1024;

        /// <summary>
        /// The buffer that is used to store recieved data
        /// </summary>
        private byte[] _Buffer = new byte[BufferSize];
        public byte[] Buffer
        {
            get { return _Buffer; }
            set { _Buffer = value; }
        }

        /// <summary>
        /// The length of the recieved data
        /// </summary>
        private int _Length = 0;
        public int Length
        {
            get { return _Length; }
            set { _Length = value; }
        }

        /// <summary>
        /// The exception that was thrown if there was an error
        /// </summary>
        public Exception Error { get; set; }
    }
}
