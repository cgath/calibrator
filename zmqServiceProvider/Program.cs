using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace zmqServiceProvider
{
    using NetMQ;
    using NetMQ.Sockets;
    using Docker.DotNet;
    using Docker.DotNet.Models;

    using zmqServiceProvider.Static;
    using zmqServiceProvider.Types;
    using zmqServiceProvider.Extensions;

    class Program
    {
        static void Main(string[] args)
        {
            //var config = new WorkerConfig
            //{ 
            //    cPort = "5555/tcp", hIP = "0.0.0.0", hPort = "5555"
            //};

            //WorkerService.RunWorker(config);
            
            /* Connect to worker and send when requested */
            using (var server = new RequestSocket())
            {
                //server.Connect("tcp://localhost:5555");

                //Console.WriteLine("ServiceProvider connected to tcp://localhost:5555");
                Console.WriteLine("Server waiting for requests. Press <Enter> to send. Press <Esc> to quit.");

                var cki = new ConsoleKeyInfo();
                string message = "Hey Worker! Do some work and get back to me would ya'?";
                List<int> freePorts = new List<int> { 4000, 4001, 4002, 4003 };

                while (true)
                {
                    cki = Console.ReadKey();
                    if (cki.Key == ConsoleKey.Enter)
                    {
                        var port = freePorts.TakeFirst();
                        var config = new WorkerConfig
                        { 
                            cPort = port+"/tcp", hIP = "0.0.0.0", hPort = port.ToString()
                        };

                        WorkerService.RunWorker(config);

                        server.Connect("tcp://localhost:"+port);
                        server.SendFrame(message);
                        
                        var response = server.ReceiveFrameString();

                        Console.WriteLine("Received from Worker: {0}", response);
                    }

                    if (cki.Key == ConsoleKey.Escape)
                    {
                        break;
                    }
                }
            }
        }
    }
}
