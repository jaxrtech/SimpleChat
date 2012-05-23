using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatLib.Protocol
{
    public abstract class ProtocolCommand
    {
        protected string Name { get; set; }

        protected ProtocolParameter[] Parameters { get; set; }

        protected string Serialize()
        {
            string command = "";

            command += Name;

            foreach (var param in Parameters)
            {
                // TODO: get the front param first and then the rest
            }

            return "";
        }
    }
}
