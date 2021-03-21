using System;
using System.Collections.Generic;
using System.Linq;

namespace ServerMon.Constructors
{
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
}