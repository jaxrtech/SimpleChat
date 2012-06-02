using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChatLib.Protocol;

namespace ProtocolTest
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("SimpleChat Protocol Serialization Tester\n");

            // Serialize
            MessageCommand command = new MessageCommand();
            command.Sender = "jaxrtech";
            command.Content = "Hello World!";

            Console.WriteLine("== Orginal");
            WriteCommandInfo(command);

            string message = CommandSerializer.Serialize(command);
            Console.WriteLine("== Serialized\n" + message);

            // Deserialize
            /* TODO: Make it be able to go into its orginal form
             *       This can probably be accomplished by writing a converter between
             *       Command -> MessageCommand or something like that or even ICommand or something
             *       So that they can be able to be converted back
             *       The use of generics may also be required too
             */ 
            Command deCommand = CommandSerializer.Deserialize(message);
            Console.WriteLine("== Desrialized");
            WriteCommandInfo(deCommand);

            Console.WriteLine("== Done");
            Console.ReadLine();
        }

        static void WriteCommandInfo(ICommand command)
        {
            Console.WriteLine(
                "Command Object " + "<" + command.GetType().ToString() + ">" + "\n" +
                "Type=" + command.Type + "\n" +
                "Name=" + command.Name + "\n" +
                "HasSender=" + command.HasSender + "\n" +
                "Sender=" + command.Sender + "\n" +
                "HasContent=" + command.HasContent + "\n" +
                "Content=" + command.Content + "\n"
            );
        }
    }
}
