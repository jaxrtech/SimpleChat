using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ChatLib;
using System.Threading;

namespace ServerTest
{
    public class Program
    {
        public static Server ServerManager;
        static ServerService Service = new ServerService();

        static void Main(string[] args)
        {
            Console.WriteLine("SimpleChat Server");
            Console.WriteLine("By: Joshua Bowden");
            Addresses();
            Service.Log("Started server");
            Thread wait = new Thread(new ThreadStart(Wait));
            Thread client = new Thread(new ThreadStart(Start));
            wait.Start();
            client.Start();
            client.Join();
        }

        static void Wait()
        {
            while (true)
            {
                Thread.Sleep(5000);
                // Report current users
                //if (Connection.Clients.Count == 0)
                //{
                //    Console.WriteLine("No clients are connected");
                //}
                //else
                //{
                //    string clients = "";
                //    foreach (var client in Connection.Clients)
                //    {
                //        clients += client.RemoteEndPoint;
                //    }
                //    Console.WriteLine("Connected clients: {0}", clients);
                //}
            }
        }

        static void Addresses()
        {
            Console.WriteLine("Local address:    " + Address.LocalIP);
            string external = "Invalid";
            if (Address.ExternalIP == null) external = "Invalid";
            else external = Address.ExternalIP.ToString();
            Console.WriteLine("External address: " + external);
        }

        static void Start()
        {
            ServerManager = new Server(Service);
            int port = Globals.Port;
            // Port number
            //Console.Write("Port (press enter for default on {0}): ", Globals.Port);
            //string port = Console.ReadLine();
            //int portNum = 0;
            //if (port == "") portNum = Globals.Port;
            //else
            //{
            //    while (!int.TryParse(port, out portNum))
            //    {
            //        Console.WriteLine("ERROR: Port invalid. Port: ");
            //        port = Console.ReadLine();
            //    }
            //}
            ServerManager.Start(port);
            string log = string.Format("Listening on port {0}", port);
            Service.Log(log);
        }
    }
}
