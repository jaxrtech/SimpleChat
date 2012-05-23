using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

namespace ChatLib
{
    public class User
    {
        public Socket Connection { get; set; }
        public string Name { get; set; }
    }
}
