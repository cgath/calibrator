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

}
