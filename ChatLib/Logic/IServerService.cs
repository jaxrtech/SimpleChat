using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatLib
{
    public interface IServerService
    {
        void OnAccept(ConnectionState state);
        void OnReceive(ConnectionState state);
        void OnDisconnect(ConnectionState state);
        void OnError(ConnectionState state);
    }
}
