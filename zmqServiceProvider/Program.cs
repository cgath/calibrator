using System;
using System.Collections.Generic;

namespace zmqServiceProvider
{
    using NetMQ;
    using NetMQ.Sockets;
    using Docker.DotNet;
    using Docker.DotNet.Models;

    class Program
    {
        static void Main(string[] args)
        {
            var dockerUri = new Uri("unix:///var/run/docker.sock");
            DockerClient docker = new DockerClientConfiguration(dockerUri).CreateClient();

            //docker run -d --name hest -p 5555:5555 aee/worker
            var image = "aee/worker";
            var name = Guid.NewGuid().ToString();

            //var ports = new Dictionary<string, EmptyStruct>();
            //ports["5555"] = new EmptyStruct();

            var parms = new CreateContainerParameters()
            {
                Image = image,
                //Name = name,
                //ExposedPorts = ports
            };

            var worker = docker.Containers.CreateContainerAsync(parms).Result.ID;

            var binding = new PortBinding()
            {
                HostIP = "localhost",
                HostPort = "5555"
            };

            var forwards = new Dictionary<string, List<PortBinding>>();
            forwards["5555"] = new List<PortBinding> { binding };

            var config = new HostConfig()
            {
                PortBindings = forwards
            };

            docker.Containers.StartContainerAsync(worker, new ContainerStartParameters());

            using (var server = new RequestSocket())
            {
                server.Connect("tcp://localhost:5555");

                Console.WriteLine("ServiceProvider connected to tcp://localhost:5555");
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
