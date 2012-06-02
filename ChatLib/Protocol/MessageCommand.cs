using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatLib.Protocol
{
    public class MessageCommand : ICommand
    {
        public const CommandType _Type = CommandType.Message;
        public const string _Name = "MSG";
        public const string[] _Arguments = null;
        public const bool _HasSender = true;
        public const bool _HasContent = true;

        /// <summary>
        /// Type of the command
        /// </summary>
        public CommandType Type
        {
            get { return _Type; } 
        }

        /// <summary>
        /// Name of the command
        /// </summary>
        public string Name
        {
            get { return _Name; }
        }

        /// <summary>
        /// Arguments of the command
        /// </summary>
        public string[] Arguments
        {
            get { return _Arguments; }
        }

        /// <summary>
        /// If the command has a sender username
        /// </summary>
        public bool HasSender
        {
            get { return true; }
        }

        /// <summary>
        /// Sender of the command
        /// </summary>
        public string Sender
        {
            get;
            set;
        }

        /// <summary>
        /// If the command has a content part
        /// </summary>
        public bool HasContent
        {
            get { return _HasContent; }
        }

        /// <summary>
        /// Content of the command
        /// </summary>
        public string Content
        {
            get;
            set;
        }
    }
}
