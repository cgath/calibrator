using System;

namespace DotnetWorker
{
    using NetMQ;
    using NetMQ.Sockets;
    
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = args[0];
            using (var send = new PushSocket(">tcp://localhost:9999"))
            using (var receive = new PullSocket(connectionString))
            {
                Console.WriteLine("Worker now listening @ {0}", connectionString);

                while (true)
                {
                    // Block for request
                    var message = receive.ReceiveFrameString();
                    Console.WriteLine("Worker received: {0}", message);

                    // Do Work ...

                    // Send response
                    var response = string.Format("Hello from worker {0}!",
                                                 connectionString);
                    Console.WriteLine("Worker sending: {0}", response);
                    send.SendFrame(response);
                }
            }
        }
    }
}
