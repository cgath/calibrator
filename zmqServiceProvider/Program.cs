﻿using System;
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
        static void HandleCalibration(List<int> ports, string model, string data)
        {
            // TODO: The ports should not be passed in. Instead the calibration
            // handler should determine the number of workers based on the model
            // and data

            var workerService = new WorkerService();

            /*
            foreach(var port in ports)
            {
                var config = new WorkerConfig
                {
                    cPort = port.ToString()+"/tcp",
                    hIP = "0.0.0.0",
                    hPort = port.ToString()
                };
            
                // Start worker
                Task.Factory.StartNew(() =>
                    workerService.StartWorker(config));
            }
            */

            var port = ports.First();
            var config = new WorkerConfig
            {
                cPort = port.ToString()+"/tcp",
                hIP = "0.0.0.0",
                hPort = port.ToString(),
                cmd = new List<string>
                {
                    "dotnet", "/tmp/published/DotnetWorker.dll", "tcp://*:"+port
                    //"python", "/tmp/worker.py", "tcp://*:"+config.cPort
                }
            };

            workerService.StartWorker(config);
        }

        static void Main(string[] args)
        {
            using (var server = new ResponseSocket())
            //using (var comm = new RequestSocket("tcp://localhost:4000"))
            //using (var poller = new NetMQPoller { comm })
            {
                //comm.ReceiveReady += (s, a) =>
                //{
                //    bool more;
                //    string messageIn = a.Socket.ReceiveFrameString(out more);
                //    Console.WriteLine("Poller received: {0}", messageIn);
                //};

                // Start polling
                //poller.RunAsync();

                Console.WriteLine("Server waiting for requests. " +
                                  "Press <Enter> to send. Press <Esc> to quit.");

                List<int> freePorts = Enumerable.Range(4000,5999).ToList();

                // TODO: Handle data and model transfer ...
                string model = "<Here goes the serialized model (protobuf rather than string?)";
                string data = "<Here goes the REDIS address of the data>";

                var cki = new ConsoleKeyInfo();
                while (true)
                {
                    cki = Console.ReadKey();
                    if (cki.Key == ConsoleKey.Enter)
                    {
                        Console.WriteLine("Request received. Delegating ...");
                        
                        var nextPorts = freePorts.TakeRange(3);
                        Task.Factory.StartNew(() => 
                            HandleCalibration(nextPorts, model, data));
                    }
/*
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
*/
                    if (cki.Key == ConsoleKey.Escape)
                    {
                        break;
                    }
                }
            }
        }
    }
}
