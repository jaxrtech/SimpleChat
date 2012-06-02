﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatLib.Protocol
{
    public class Command : ICommand
    {
        /// <summary>
        /// Type of the command
        /// </summary>
        public CommandType Type { get; set; }

        /// <summary>
        /// Name of the command
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Arguments of the command
        /// </summary>
        public string[] Arguments { get; set; }

        /// <summary>
        /// If the command has a sender username
        /// </summary>
        public bool HasSender { get; set; }

        /// <summary>
        /// Sender of the command
        /// </summary>
        public string Sender { get; set; }

        /// <summary>
        /// If the command has a content part
        /// </summary>
        public bool HasContent { get; set; }

        /// <summary>
        /// Content of the command
        /// </summary>
        public string Content { get; set; }
    }
}