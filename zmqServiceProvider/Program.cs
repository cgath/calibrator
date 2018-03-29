using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace zmqServiceProvider
{
    using NetMQ;
    using NetMQ.Sockets;

    using zmqServiceProvider.Types;
    using zmqServiceProvider.Extensions;
    using zmqServiceProvider.Workers;

    class Program
    {
        static void Main(string[] args)
        {
            /* Connect to worker and send when requested */
            using (var server = new ResponseSocket())
            {
                Console.WriteLine("Server waiting for requests. " +
                                  "Press <Enter> to send. Press <Esc> to quit.");

                List<int> freePorts = Enumerable.Range(4000,5999).ToList();
                List<Worker> workers = new List<Worker>();

                var cki = new ConsoleKeyInfo();
                string message = "Hey Worker! Do some work and get back to me would ya'?";
                while (true)
                {
                    cki = Console.ReadKey();
                    if (cki.Key == ConsoleKey.Enter)
                    {
                        Console.WriteLine("Request received. Delegating ...");
                        var nextPort = freePorts.TakeFirst();

                        Task.Factory.StartNew((port) =>
                        {
                            var config = new WorkerConfig
                            {
                                // Set image = xxxx to use different worker ...
                                cPort = port.ToString()+"/tcp",
                                hIP = "0.0.0.0",
                                hPort = port.ToString()
                            };

                            // Start worker
                            var worker = WorkerService.RunWorker(config);

                            using(var socket = new RequestSocket())
                            {
                                if (worker.runStatus)
                                {
                                    Console.WriteLine("Worker running. Connecting to port {0}", port);
                                
                                    socket.Connect("tcp://localhost:"+port);
                                    socket.SendFrame(message);

                                    var response = socket.ReceiveFrameString();
                                    Console.WriteLine("Received from Worker: {0}", response);
                                }
                                else
                                {
                                    Console.WriteLine("Worker not running ...");
                                }
                            }

                            workers.Add(worker);
                        }, nextPort);
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
