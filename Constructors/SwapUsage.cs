using System;
using System.Collections.Generic;
using System.Linq;

namespace ServerMon.Constructors
{
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