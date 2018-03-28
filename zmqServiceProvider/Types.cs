using System;

namespace zmqServiceProvider.Types
{
    using Docker.DotNet;
    using Docker.DotNet.Models;

    public class WorkerConfig
    {
        public string image = "aee/worker-dotnet";
        public string cPort;
        public string hIP;
        public string hPort;
    }

    public class Worker
    {
        public WorkerConfig config;

        public CreateContainerResponse createStatus;

        public bool runStatus;
    }
}
