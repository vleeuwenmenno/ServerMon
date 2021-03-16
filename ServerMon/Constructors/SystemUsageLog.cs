using System;
using SQLite;

namespace ServerMon.Constructors
{
    [Table("SystemUsageLog")]
    public class SystemUsageLog
    {
        [PrimaryKey]
        public DateTime timestamp {get;set;}

        public decimal user {get;set;}
        public decimal system {get;set;}
        public decimal wait {get;set;}
        public decimal idle {get;set;}

        public long totalMemory {get;set;}
        public long usedMemory {get;set;}
        public long freeMemory {get;set;}

        public long totalSwap {get;set;}
        public long usedSwap {get;set;}
        public long freeSwap {get;set;}
    }
}