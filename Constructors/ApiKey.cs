using System;
using FreeSql.DataAnnotations;

namespace ServerMon.Constructors
{
    public class ApiKey
    {
        [Column(IsPrimary = true, IsIdentity = false)]
        public int id {get;set;}
        public string key {get;set;}
        public DateTime expiry {get;set;}
    }
}