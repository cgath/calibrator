using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace zmqServiceProvider
{
    using NetMQ;
    using NetMQ.Sockets;

    using zmqServiceProvider.Static;
    using zmqServiceProvider.Types;
    using zmqServiceProvider.Extensions;
    using zmqServiceProvider.Workers;

    class Program
    {
        static void HandleCalibration(int port, string model, string data)
        {
            Console.WriteLine("Starting CalibrationHandler ...");

            var workerService = new WorkerService();

            var config = new WorkerConfig
            {
                cPort = port.ToString()+"/tcp",
                hIP = "0.0.0.0",
                hPort = port.ToString(),
                cmd = new List<string>
                {
                    "dotnet", "/tmp/published/CalibrationHandler.dll", "tcp://0.0.0.0:"+port
                    //"python", "/tmp/worker.py", "tcp://*:"+config.cPort
                }
            };

            // Start worker
            workerService.StartWorker(config);

            // Push task to worker and wait for ack
            using (var worker = new RequestSocket("tcp://127.0.0.1:"+port))
            {
                // TODO: Serialize <model, data> ...

                Console.WriteLine("Pushing to {0}", "tcp://localhost:"+port);
                worker.SendFrame("Hey worker, do some work and get back to us ok?");

                var response = worker.ReceiveFrameString();
                Console.WriteLine("Calibration handler received: {0}", response);
            }

            // After ack from worker exit thread and let server collect
            // finished tasks ...
        }

        static void Main(string[] args)
        {
            using (var server = new ResponseSocket())
            using (var comm = new PullSocket("@tcp://*:9999"))
            using (var poller = new NetMQPoller{ comm })
            {
                comm.ReceiveReady += (s, a) =>
                {
                    //bool more;
                    //string messageIn = a.Socket.ReceiveFrameString(out more);
                    string message = a.Socket.ReceiveFrameString();
                    
                    Console.WriteLine("Poller ==> Received: {0}", message);

                    // TODO: When receiving a message send response to caller
                };

                // Start polling
                poller.RunAsync();

                Console.WriteLine("Server waiting for requests. " +
                                  "Press <Spacebar> to generate test data. " +
                                  "Press <Enter> to send. " + 
                                  "Press <Esc> to quit.");

                List<int> freePorts = Enumerable.Range(4000,5999).ToList();

                // TODO: Handle data and model transfer ...
                string model = "<Here goes the serialized model (protobuf?)";
                string data = Guid.NewGuid().ToString();

                var cki = new ConsoleKeyInfo();
                while (true)
                {
                    cki = Console.ReadKey();
                    if (cki.Key == ConsoleKey.Spacebar)
                    {
                        Console.Write("Generating test data ... ");
                        var filename =  data + ".dat";
                        Data.GenerateDataFile(filename, 40000, 4200);
                        Console.WriteLine("Done!");
                    }
                    if (cki.Key == ConsoleKey.Enter)
                    {
                        Console.WriteLine("Request received. Delegating ...");
                        
                        var port = freePorts.TakeFirst();
                        Task.Factory.StartNew(() => 
                            HandleCalibration(port, model, data));
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
