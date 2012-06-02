using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ChatLib.Protocol
{
    public class CommandSerializer
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

        public bool Validate(ICommand command)
        {
            if (command.HasSender && string.IsNullOrEmpty(command.Sender))
                return false;
            else if (command.HasContent && string.IsNullOrEmpty(command.Content))
                return false;
            else
                return true;
        }

        public static string Serialize(ICommand command)
        {
            string message = string.Empty;
            char space = ' ';

            // Append sender
            if (command.HasSender)
                message += SenderPrefix + command.Sender + space;

            // Append command name
            message += command.Name;

            // Append each argument
            if (command.Arguments != null)
            {
                foreach (var arg in command.Arguments)
                    message += space + arg;
            }

            // Append content
            if (command.HasContent)
                message += ContentPrefix + command.Content;

            // Append line ending
            message += LineEnding;

            return message;
        }

        /* TODO: Make sure that one command does in at a time
         *       For this reason, it may be better to make annother method that splits them at the line ending and deals with them seperatly
         *       by calling this method instead. This method should then thrown an error or return null if there are more than on line ending
         *       in it instead of just the one at the end.
         */ 
        public static Command Deserialize(string message)
        {
            // Check and format the string
            try
            {
                // Make sure that it is not blank
                if (string.IsNullOrWhiteSpace(message))
                    return null;
                else if (message.EndsWith(LineEnding))
                    // Remove line ending since it it there for good measure
                    /* TODO: Need to make sure that it replace any one before the end with just a \n and
                       just to search at the ending 2 chars instead */
                    message = message.Replace(LineEnding, string.Empty);
            }
            catch
            {
                return null;
            }

            Command command = new Command();
            char space = ' ';

            // Split the content
            List<string> parts = message.Split(new string[] { ContentPrefix }, 2, StringSplitOptions.RemoveEmptyEntries).ToList<string>();

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
                    command.Content = parts[1];
                    parts.RemoveAt(1);
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
                args[0] = args[0].Replace(SenderPrefix, string.Empty);
                command.Sender = args[0];
                args.RemoveAt(0);
            }

            // Set the command name
            args[0].ToUpper();
            command.Name = args[0];
            args.RemoveAt(0);

            // Set the rest of the arguments
            command.Arguments = args.ToArray();

            // Remove the first part
            parts.RemoveAt(0);

            // Return the correct type
            switch (command.Name)
            {
                case MessageCommand._Name:
                    command.Type = CommandType.Message;
                    break;
                default:
                    return null;
            }

            return command;
        }

    }
}
