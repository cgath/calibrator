using System;
using System.Threading;

namespace CalibrationHandler
{
    using NetMQ;
    using NetMQ.Sockets;
    
    class Program
    {
        static void Main(string[] args)
        {
            string connectionString = args[0];
            using (var send = new PushSocket(">tcp://127.0.0.1:9999"))
            using (var receive = new ResponseSocket(connectionString))
            {
                while (true)
                {
                    // Block for request
                    var message = receive.ReceiveFrameString();
                    var msgId = Guid.NewGuid().ToString();

                    // Acknowledge
                    receive.SendFrame("200, ACK");

                    // Do Work ...
                    Thread.Sleep(2000);

                    // Send response
                    send.SendFrame(string.Format("Handler {0} completed task {1}",
                                                 connectionString, msgId));

                    // Exit handler
                    break;
                }
            }
        }
    }
}
