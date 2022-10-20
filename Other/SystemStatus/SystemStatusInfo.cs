using System;
using System.Diagnostics;
using System.Management;

namespace HDF.Windows.Tools.Other.SystemStatus
{



    public sealed class SystemStatusInfo
    {
        private SystemStatusInfo() { }

        public static RAMInfo RAM => RAMInfo.Instance;
        public static CPUInfo CPU => CPUInfo.Instance;







        public sealed class CPUInfo
        {
            public static CPUInfo Instance = new CPUInfo();






            private string cpuName;

            private PerformanceCounter pcCpuLoad;

            private PerformanceCounter[] pcCpuCoreLoads;

            public string CpuName => cpuName.Trim();

            public int ProcessorCount { get; } = Environment.ProcessorCount;


            public float CpuLoad => pcCpuLoad.NextValue();

            private CPUInfo()
            {

                pcCpuLoad = new PerformanceCounter("Processor", "% Processor Time", "_Total");
                pcCpuCoreLoads = new PerformanceCounter[ProcessorCount];
                for (int i = 0; i < ProcessorCount; i++)
                {
                    pcCpuCoreLoads[i] = new PerformanceCounter("Processor", counterName: "% Processor Time", i.ToString());
                }
                pcCpuLoad.MachineName = ".";
                pcCpuLoad.NextValue();
                for (int j = 0; j < ProcessorCount; j++)
                {
                    pcCpuCoreLoads[j].NextValue();
                }
                string text = string.Empty;
                ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("Select Name from Win32_Processor");
                foreach (ManagementBaseObject item in managementObjectSearcher.Get())
                {
                    ManagementObject managementObject = (ManagementObject)item;
                    text = managementObject["Name"].ToString();
                }
                cpuName = text;
            }

            public float CpuCoreLoad(int core_num)
            {
                return pcCpuCoreLoads[core_num].NextValue();
            }

        }


        public sealed class RAMInfo
        {

            public static RAMInfo Instance = new RAMInfo();





            public long ComputerAllMemory { get; init; }

            public string ComputerAllMemoryString { get; init; }




            private PerformanceCounter currentMenory;

            public long ComputerCurrentMemory => ComputerAllMemory - (long)Math.Round(currentMenory.NextValue());
            public string ComputerCurrentMemoryString => GetRAMString(ComputerCurrentMemory);



            public float ComputerMemoryOccupancy => ComputerCurrentMemory / (float)ComputerAllMemory;
            public string ComputerMemoryOccupancyString => $"{ComputerMemoryOccupancy * 100}%";






            public long ProcessWokingSet => Process.GetCurrentProcess().WorkingSet64;
            public string ProcessWokingSetString => GetRAMString(ProcessWokingSet);


            private PerformanceCounter processWokingMenory;

            public long ProcessWokingMenory => (long)Math.Round(processWokingMenory.NextValue());
            public string ProcessWokingMenoryString => GetRAMString(ProcessWokingMenory);




            private PerformanceCounter processWokingPrivateMenory;

            public long ProcessWokingPrivateMenory => (long)Math.Round(processWokingPrivateMenory.NextValue());
            /// <summary>
            /// 同任务管理器中显示
            /// </summary>
            public string ProcessWokingPrivateMenoryString => GetRAMString(ProcessWokingPrivateMenory);







            private RAMInfo()
            {
                ManagementObjectCollection instances = new ManagementClass("Win32_ComputerSystem").GetInstances();
                foreach (ManagementObject item in instances)
                {
                    if (item["TotalPhysicalMemory"] != null)
                    {
                        ComputerAllMemory = long.Parse(item["TotalPhysicalMemory"].ToString());
                        ComputerAllMemoryString = GetRAMString(ComputerAllMemory);
                    }
                }


                currentMenory = new PerformanceCounter("Memory", "Available Bytes");

                var ps = Process.GetCurrentProcess();
                processWokingMenory = new PerformanceCounter("Process", "Working Set", ps.ProcessName);
                processWokingPrivateMenory = new PerformanceCounter("Process", "Working Set - Private", ps.ProcessName);


            }

            private string GetRAMString(long num)
            {
                int ramScale = (int)Math.Floor(Math.Log(num, 1024.0));
                double memTotal = Math.Round(num / Math.Pow(1024, ramScale), 2);

                if (ramScale == 0)
                    return memTotal + "byte";
                else if (ramScale == 1)
                    return memTotal + "KB";
                else if (ramScale == 2)
                    return memTotal + "MB";
                else if (ramScale == 3)
                    return memTotal + "GB";
                else if (ramScale == 4)
                    return memTotal + "TB";

                return memTotal.ToString();
            }
        }




    }



}

