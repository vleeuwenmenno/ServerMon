using System;
using FreeSql.DataAnnotations;

namespace ServerMon.Constructors
{
    public class SystemUsageLog
    {
        [Column(IsPrimary = true, IsIdentity = false)]
        public DateTime timestamp {get;set;}

        public decimal user {get;set;}
        public decimal nice {get;set;}
        public decimal system {get;set;}
        public decimal iowait {get;set;}
        public decimal steal {get;set;}
        public decimal idle {get;set;}

        public long totalMemory {get;set;}
        public long usedMemory {get;set;}
        public long freeMemory {get;set;}

        public long totalSwap {get;set;}
        public long usedSwap {get;set;}
        public long freeSwap {get;set;}
    }
}