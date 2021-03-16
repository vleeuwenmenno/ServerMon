using System;
using System.Linq;
using SQLite;

namespace Auth
{
    [Table("APIToken")]
    public class APIToken
    {
        [PrimaryKey]
        public string id {get;set;}
        public DateTime expiry {get;set;}
    }
}