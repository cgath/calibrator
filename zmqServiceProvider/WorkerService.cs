using System;
using System.Collections.Generic;

namespace zmqServiceProvider.Workers
{
    using zmqServiceProvider.Types;
    using zmqServiceProvider.Static;

    using Docker.DotNet;
    using Docker.DotNet.Models;

    public class WorkerService
    {
        public WorkerService()
        {
            this.client = GetDockerClient();
        }

        public void StartWorker(WorkerConfig config)
        {
            try
            {
                RunContainer(CreateContainer(config));
            }
            catch (Exception e)
            {
                throw new Exception(
                    string.Format("ERROR: Worker initialization failed ==> {0}",
                                  e.Message));
            }
        }

        private DockerClient client;

        private DockerClient GetDockerClient()
        {
            var dockerUriString = "unix:///var/run/docker.sock";
            if (OS.IsWindows()) dockerUriString = "npipe://./pipe/docker_engine";

            var dockerUri = new Uri(dockerUriString);
            return new DockerClientConfiguration(dockerUri).CreateClient();
        }

        private Worker CreateContainer(WorkerConfig config)
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
                        config.cPort, new List<PortBinding>
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
                HostConfig = hostConfig,
                Cmd = cmd
            };

            var status = client.Containers
                               .CreateContainerAsync(createParams)
                               .Result;

            return new Worker()
            {
                config = config,
                createStatus = status,
                runStatus = false
            };

            //client.Containers.CreateContainerAsync(createParams);
        }

        private void RunContainer(Worker worker)
        {
            var start = new ContainerStartParameters();
            var id = worker.createStatus.ID;

            worker.runStatus = client.Containers
                                     .StartContainerAsync(id, start)
                                     .Result;
            
            //return worker;

            //client.Containers.StartContainerAsync(id, start);
        }
    }
}