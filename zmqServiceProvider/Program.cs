using System;

namespace zmqServiceProvider
{
    using NetMQ;
    using NetMQ.Sockets;

    class Program
    {
        static void Main(string[] args)
        {
            using (var server = new RequestSocket())
            {
                server.Connect("tcp://localhost:5555");

                Console.WriteLine("Press <Enter> to send. Press <Esc> to quit.");

                var cki = new ConsoleKeyInfo();
                string message = "Hey Worker! Do some work and get back to me would ya'?";
                while (true)
                {
                    cki = Console.ReadKey();
                    if (cki.Key == ConsoleKey.Enter)
                    {
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
