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
            using (var socket = new ResponseSocket())
            {
                socket.Bind(connectionString);
                Console.WriteLine("Worker listening @ {0}", connectionString);

                while (true)
                {
                    var message = socket.ReceiveFrameString();
                    
                    // Do Work ...

                    // Send response
                    socket.SendFrame("Hello from worker!");
                }
            }
        }
    }
}
