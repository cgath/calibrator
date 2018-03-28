using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace zmqServiceProvider.Static
{
    using Docker.DotNet;
    using Docker.DotNet.Models;

    using zmqServiceProvider.Types;

    public static class OS
    {
        public static bool IsWindows() => 
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        
        public static bool IsOSX() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        
        public static bool IsLinux() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    }

    public static class WorkerService
    {
        public static Worker RunWorker(WorkerConfig config)
        {
            try
            {
                return RunContainer(CreateContainer(config));
            }
            catch (Exception)
            {
                throw new Exception("ERROR: Worker initialization failed ...");
            }
        }

        private static DockerClient client = GetDockerClient();
        
        private static DockerClient GetDockerClient()
        {
            var dockerUriString = "unix:///var/run/docker.sock";
            if (OS.IsWindows()) dockerUriString = "npipe://./pipe/docker_engine";

            var dockerUri = new Uri(dockerUriString);
            return new DockerClientConfiguration(dockerUri).CreateClient();
        }

        private static Worker CreateContainer(WorkerConfig config)
        {
            var exposedPorts = new Dictionary<string, EmptyStruct>
            {
                { config.hPort, new EmptyStruct() }
            };

            var hostConfig = new HostConfig
            {
                PortBindings = new Dictionary<string, IList<PortBinding>>()
                {
                    {
                        config.cPort,
                        new List<PortBinding>
                        {
                            new PortBinding
                            { 
                                HostPort = config.hPort
                            }
                        }
                    }
                }
            };

            var cmd = new List<string>
            {
                "dotnet", "/tmp/published/DotnetWorker.dll", "tcp://*:"+config.hPort
                //"python", "/tmp/worker.py", "tcp://*:"+config.cPort
            };

            var createParams = new CreateContainerParameters
            {
                Image = config.image,
                //ExposedPorts = exposedPorts,
                HostConfig = hostConfig,
                Cmd = cmd
            };

            var status = client.Containers.CreateContainerAsync(createParams).Result;

            return new Worker()
            {
                config = config,
                createStatus = status,
                runStatus = false
            };
        }

        private static Worker RunContainer(Worker worker)
        {
            var start = new ContainerStartParameters();
            var id = worker.createStatus.ID;

            worker.runStatus = client.Containers
                                     .StartContainerAsync(id, start)
                                     .Result;
            
            return worker;
        }
    }
}
