using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatLib.Protocol
{
    public class MessageCommand : Command
    {
        /// <summary>
        /// Name of the command
        /// </summary>
        public new const string Name = "MSG";

        /// <summary>
        /// Arguments of the command
        /// </summary>
        public new const string[] Arguments = null;

        /// <summary>
        /// If the command has a sender username
        /// </summary>
        public new bool HasSender = true;

        /// <summary>
        /// If the command has a content part
        /// </summary>
        public new bool HasContent = true;
    }
}
