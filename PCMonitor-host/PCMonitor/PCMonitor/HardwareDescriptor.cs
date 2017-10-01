using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PCMonitor
{
    public static class HardwareDescriptor
    {
        //public static bool isUpdated;
        
        public static string MainboardName;

        public static string CpuName;
        public static float CpuTotalLoad;
        public static float CpuCore0Load;
        public static float CpuCore1Load;
        public static float CpuCore2Load;
        public static float CpuCore3Load;
        public static float CpuCore0Clock;
        public static float CpuCore1Clock;
        public static float CpuCore2Clock;
        public static float CpuCore3Clock;
        public static float CpuTemp;
        public static float CpuBusLoad;

        public static string RamName;
        public static float RamLoad;
        public static float RamUsedMem;
        public static float RamFreeMem;

        public static string GpuName;
        public static float GpuTemp;
        public static float GpuFan;
        public static float GpuFanCtrl;
        public static float GpuCoreClock;
        public static float GpuMemClock;
        public static float GpuVolts;
        public static float GpuLoad;

        public static string SsdSysName;
        public static float SsdSysTemp;
        public static float SsdSysLoad;

        public static string SsdGameName;
        public static float SsdGameLoad;

        public static string HddName;
        public static float HddTemp;
        public static float HddLoad;

    }
}
