using System;
using FreeSql.DataAnnotations;

namespace ServerMon.Constructors
{
    public class APIToken
    {
        [Column(IsPrimary = true, IsIdentity = false)]
        public string id {get;set;}
        public DateTime expiry {get;set;}
    }
}