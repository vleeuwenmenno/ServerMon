using System;

namespace ServerMon.Constructors
{
    public class SystemUsage
    {
        public MemoryUsage memory {get;set;}
        public SwapUsage swap {get;set;}
        public CpuUsage cpu {get;set;}

        public DateTime dateTime;

        public static SystemUsage profile()
        {
            SystemUsage systemUsage = new SystemUsage();

            systemUsage.cpu = CpuUsage.profile();
            systemUsage.memory = MemoryUsage.profile();
            systemUsage.swap = SwapUsage.profile();
            systemUsage.dateTime = DateTime.UtcNow;

            return systemUsage;
        }
    }
}