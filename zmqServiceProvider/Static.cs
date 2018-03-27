using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

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
        public static bool RunWorker(WorkerConfig config)
        {
            try
            {
                return RunContainer(CreateContainer(config).runStatus.ID);
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
            var hostConfig = new HostConfig
            {
                PortBindings = new Dictionary<string, IList<PortBinding>>()
                {
                    {
                        config.cPort, new List<PortBinding>() {
                            new PortBinding()
                            { 
                                HostIP = config.hIP,
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
                HostConfig = hostConfig,
                //Cmd = cmd
            };

            var status = client.Containers.CreateContainerAsync(createParams).Result;

            return new Worker()
            {
                config = config,
                runStatus = status
            };
        }

        private static bool RunContainer(string id)
        {
            var start = new ContainerStartParameters();

            return client
                   .Containers
                   .StartContainerAsync(id, start)
                   .IsCompletedSuccessfully;
        }
    }
}
