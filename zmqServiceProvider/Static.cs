using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace zmqServiceProvider.Static
{
    public static class OS
    {
        public static bool IsWindows() => 
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
        
        public static bool IsOSX() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
        
        public static bool IsLinux() =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
    }

    public static class Data
    {
        public static bool GenerateDataFile(string filename, int nSamples, int nPoints)
        {
            var rng = new Random();
            var sample = new double[nPoints];

            var mode = FileMode.Create;
            var access = FileAccess.Write;

            using (FileStream fs = new FileStream(filename, mode, access))
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                for (int i=0; i < nSamples; i++)
                {
                    // Generate next sample
                    for (int j = 0; j < nPoints; j++)
                    {
                        sample[j] = rng.NextDouble();
                    }

                    var bytes = new byte[sample.Length * sizeof(double)];
                    Buffer.BlockCopy(sample, 0, bytes, 0, bytes.Length);

                    // Write sample to file
                    writer.Write(bytes);
                }
            }

            return true;
        }
    }

}
