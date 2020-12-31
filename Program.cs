using System;
using System.Diagnostics;
using System.Threading;
using System.Dynamic;
using System.ServiceProcess;

namespace Auramonitor
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceController service = new ServiceController("LightingService");

            while (true)
            {
                dynamic result = PerfMonitor();

                //Console.WriteLine(result.CPU);
                //Console.WriteLine(result.RAM);
                if (result.CPU > 10)
                {
                    service.Stop();
                    service.WaitForStatus(ServiceControllerStatus.Stopped);
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running);
                }
                else
                {
                }
            }
        }

        private static dynamic PerfMonitor()
        {
            var cpu = new PerformanceCounter("Process", "% Processor Time", "LightingService", true);
            //var ram = new PerformanceCounter("Process", "Private Bytes", "LightingService", true);

            // Getting first initial values
            cpu.NextValue();
            //ram.NextValue();

            // Creating delay to get correct values of CPU usage during next query
            Thread.Sleep(1000);

            dynamic result = new ExpandoObject();

            // If system has multiple cores, that should be taken into account
            result.CPU = Math.Round(cpu.NextValue() / Environment.ProcessorCount, 2);
            //Returns number of MB consumed by application
            //result.RAM = Math.Round(ram.NextValue() / 1024 / 1024, 2);
            return result;
        }
    }
}
