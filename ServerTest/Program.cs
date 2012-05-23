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
        const string ProductName = "SimepleChat Server";
        const string Author = "Joshua Bowden";

        public static Server ServerManager;
        static ServerService Service = new ServerService();
        static int Port;

        static void Main(string[] args)
        {
            Console.Title = ProductName;
            Console.WriteLine(ProductName);
            Console.WriteLine("By: " + Author);

            Service.Log("Started server");
            Addresses();
            
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
            Service.Log("Local address:    " + Address.LocalIP);
            string external = "Invalid";
            if (Address.ExternalIP == null) 
                external = "Invalid";
            else 
                external = Address.ExternalIP.ToString();
            Service.Log("External address: " + external);
        }

        static void Start()
        {
            ServerManager = new Server(Service);
            //int port = Globals.Port;
            // Port number
            Console.Write("Port (press enter for default on {0}): ", Globals.Port);
            string portString = Console.ReadLine();
            if (portString == "")
            {
                Port = Globals.Port;
            }
            else
            {
                while (!int.TryParse(portString, out Port))
                {
                    Console.WriteLine("ERROR: Port invalid. Port: ");
                    portString = Console.ReadLine();
                }
            }

            // Start
            Console.Title = ProductName + " - " + Port;
            ServerManager.Start(Port);
            Service.Log(string.Format("Listening on port {0}", Port));
        }
    }
}
