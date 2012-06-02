using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatLib.Protocol
{
    public interface ICommand
    {
        /// <summary>
        /// Type of the command
        /// </summary>
        CommandType Type { get; }

        /// <summary>
        /// Name of the command
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Arguments of the command
        /// </summary>
        string[] Arguments { get; }

        /// <summary>
        /// If the command has a sender username
        /// </summary>
        bool HasSender { get; }

        /// <summary>
        /// Sender of the command
        /// </summary>
        string Sender { get; }

        /// <summary>
        /// If the command has a content part
        /// </summary>
        bool HasContent { get; }

        /// <summary>
        /// Content of the command
        /// </summary>
        string Content { get; }
    }
}
