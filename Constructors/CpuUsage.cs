using System.Collections.Generic;
using System.Linq;

namespace ServerMon.Constructors
{
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
            return CpuUsage.parse("sar 1 4".Bash().Split("\n").ToList());
        }
    }
}