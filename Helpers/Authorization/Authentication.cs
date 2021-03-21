using System;
using System.Collections.Generic;
using System.Linq;
using ServerMon.Constructors;

namespace ServerMon.Helpers.Authorization
{
    public class Authentication
    {
        public static bool VerifyAPIAccess(string apiToken, IFreeSql db)
        {
            if (string.IsNullOrEmpty(apiToken))
                return false; 

            ApiKey at = db.Select<ApiKey>().Where(a => a.key == apiToken).ToOne();

            if (at == null)
                return false;
            else
            {
                //Check if the session is expired;
                if (at.expiry < DateTime.UtcNow)
                    return false;
                
                else
                    return true;
            }
        }
    }
}