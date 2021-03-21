using System;
using System.Linq;
using FreeSql.DataAnnotations;

namespace Auth
{
    public class APIToken
    {
        [Column(IsPrimary = true, IsIdentity = false)]
        public string id {get;set;}
        public DateTime expiry {get;set;}
    }
}