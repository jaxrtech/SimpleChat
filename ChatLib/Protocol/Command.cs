using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatLib.Protocol
{
    public class Command
    {
        /// <summary>
        /// Prefix used to start the sender arguments
        /// </summary>
        public const string SenderPrefix = ":";

        /// <summary>
        /// Prefix used to start the content arguments
        /// </summary>
        public const string ContentPrefix = " :";

        /// <summary>
        /// Line ending at each message (CRLF)
        /// </summary>
        public const string LineEnding = "\r\n";

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

        public string Serialize()
        {
            string message = string.Empty;
            char space = ' ';

            // Append sender
            if (this.HasSender)
                message += SenderPrefix + this.Sender + space;

            // Append command name
            message += this.Name;

            // Append each argument
            foreach (var arg in this.Arguments)
                message += space + arg;

            // Append content
            if (this.HasContent)
                message += ContentPrefix + this.Content;

            // Append line ending
            message += LineEnding;

            return message;
        }

        public Command Deserialize(string message)
        {
            // Check and format the string
            try
            {
                // Make sure that it is not blank
                if (string.IsNullOrWhiteSpace(message))
                    return null;
                else if (message.EndsWith(LineEnding))
                    // Remove line ending since it it there for good measure
                    message.LastIndexOf(LineEnding, message.Length - 2, 1, StringComparison.OrdinalIgnoreCase);
            }
            catch
            {
                return null;
            }

            Command command = new Command();
            char space = ' ';
            
            // Split the content
            List<string> parts = message.Split(ContentPrefix.ToCharArray(), 2).ToList<string>();

            // Check if the content part exists or not
            switch (parts.Count)
            {
                case 0: // this should never happen from the first check
                    return null;
                case 1: // no content param
                    command.HasContent = false;
                    break;
                case 2: // has content param
                    command.HasContent = true;
                    break;
                default:
                    return null;
            }

            // Split rest of params
            List<string> args = parts[0].Split(space).ToList<string>();

            // Check if the first param is the sender name
            if (args[0].StartsWith(SenderPrefix))
            {
                command.HasSender = true;
                args[0].Replace(SenderPrefix, string.Empty);
                command.Sender = args[0];
                args.RemoveAt(0);
            }

            // Set the command name
            args[0].ToUpper();
            command.Name = args[0];
            args.RemoveAt(0);

            // Set the rest of the arguments
            command.Arguments = args.ToArray();

            // Return the correct type
            switch (command.Name)
            {
                case MessageCommand.Name:
                    return (MessageCommand)command;
                default:
                    return null;
            }
        }
    }
}
