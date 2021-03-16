using System;
using System.Collections.Generic;
using System.Linq;
using SQLite;

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

    public class CpuUsage
    {
        public decimal user {get;set;}
        public decimal nice {get;set;}
        public decimal system {get;set;}
        public decimal iowait {get;set;}
        public decimal steal {get;set;}
        public decimal idle {get;set;}

        public static CpuUsage parse(List<string> str)
        {
            foreach (string s in str)
            {
                if (s.StartsWith("Average:"))
                {
                    CpuUsage cpu = new CpuUsage();
                    List<decimal> workingSet = s.Replace(",", ".").Split("     ").Where(x => decimal.TryParse(x.Trim(), out decimal d)).Select(x => decimal.Parse(x.Trim())).ToList();
                    
                    cpu.user = workingSet[0];
                    cpu.nice = workingSet[1];
                    cpu.system = workingSet[2];
                    cpu.iowait = workingSet[3];
                    cpu.steal = workingSet[4];
                    cpu.idle = workingSet[5];

                    return cpu;
                }
            }
            return null;
        }

        public static CpuUsage parse(string str)
        {
            return CpuUsage.parse(str.Split('\n').ToList());
        }

        public static CpuUsage profile()
        {
            return CpuUsage.parse("sar 1 5".Bash().Split("\n").ToList());
        }
    }

    public class MemoryUsage
    {
        public long totalMemory {get;set;}
        public long usedMemory {get;set;}
        public long freeMemory {get;set;}

        public static MemoryUsage parse(List<string> str)
        {
            MemoryUsage mem = new MemoryUsage();
            foreach (string s in str)
            {
                if (s.EndsWith("K total memory"))
                    mem.totalMemory = long.Parse(s.Replace("K total memory", ""));

                else if (s.EndsWith("K used memory"))
                    mem.usedMemory = long.Parse(s.Replace("K used memory", ""));

                else if (s.EndsWith("K free memory"))
                    mem.freeMemory = long.Parse(s.Replace("K free memory", ""));
            }
            return mem;
        }

        public static MemoryUsage parse(string str)
        {
            return MemoryUsage.parse(str.Split('\n').ToList());
        }

        public static MemoryUsage profile()
        {
            return MemoryUsage.parse("vmstat -s".Bash().Split('\n').Select(x => x.Trim()).ToList());
        }            
    }

    public class SwapUsage
    {
        public long totalSwap {get;set;}
        public long usedSwap {get;set;}
        public long freeSwap {get;set;}

        public static SwapUsage parse(List<string> str)
        {
            SwapUsage swap = new SwapUsage();
            foreach (string s in str)
            {
                if (s.EndsWith("K total swap"))
                    swap.totalSwap = long.Parse(s.Replace("K total swap", ""));

                else if (s.EndsWith("K used swap"))
                    swap.usedSwap = long.Parse(s.Replace("K used swap", ""));

                else if (s.EndsWith("K free swap"))
                    swap.freeSwap = long.Parse(s.Replace("K free swap", ""));
            }
            return swap;
        }

        public static SwapUsage parse(string str)
        {
            return SwapUsage.parse(str.Split('\n').ToList());
        }

        public static SwapUsage profile()
        {
            return SwapUsage.parse("vmstat -s".Bash().Split('\n').Select(x => x.Trim()).ToList());
        }  
    }
}