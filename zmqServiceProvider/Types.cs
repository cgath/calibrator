using System;
using System.Collections.Generic;

namespace zmqServiceProvider.Types
{
    using Docker.DotNet;
    using Docker.DotNet.Models;

    public class WorkerConfig
    {
        public string image = "aee/worker-dotnet:latest";
        public string cPort;
        public string hIP;
        public string hPort;
        public List<string> cmd;
    }

    public class Worker
    {
        public WorkerConfig config;

        public CreateContainerResponse createStatus;

        public bool runStatus;
    }
}
