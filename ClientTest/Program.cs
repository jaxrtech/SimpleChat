using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using ChatLib;

namespace ClientTest
{
    public class Program
    {
        static Client Connection;
        static ClientService Service;

        static void Main(string[] args)
        {
            Thread wait = new Thread(new ThreadStart(Wait));
            Thread client = new Thread(new ThreadStart(Start));
            wait.Start();
            client.Start();
        }

        static void Wait()
        {
            while (true)
            {
                Thread.Sleep(500);
            }
        }

        static void Start()
        {
            Service = new ClientService();
            Connection = new Client(Service);
            Console.Write("Server address: ");
            string ip = Console.ReadLine();
            Console.Write("Server port: ");
            string port = Console.ReadLine();
            int portNum = int.Parse(port);
            Connection.Start(ip, portNum);
        }
    }
}
