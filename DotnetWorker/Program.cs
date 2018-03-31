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
            using (var comm = new RequestSocket("tcp://localhost:3333"))
            using (var socket = new ResponseSocket())
            {
                socket.Bind(connectionString);
                Console.WriteLine("Worker listening @ {0}", connectionString);

                while (true)
                {
                    var message = socket.ReceiveFrameString();
                    
                    // Do Work ...

                    // Send response
                    //var response = string.Format("Hello from worker {0}!", connectionString);
                    //socket.SendFrame(response);

                    var response = string.Format("Hello from worker {0}!", connectionString);
                    comm.SendFrame(response);
                }
            }
        }
    }
}
