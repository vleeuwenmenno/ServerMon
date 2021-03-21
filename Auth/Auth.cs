using System;
using System.Collections.Generic;
using System.Linq;

namespace Auth
{
    public class Authentication
    {
        public static Options options = new Options();
        public static bool VerifyAPIAccess(string apiToken, IFreeSql db)
        {
            if (string.IsNullOrEmpty(apiToken) || !apiToken.StartsWith("Bearer"))
                return false; 

            apiToken = apiToken.Replace("Bearer", "").Trim();

            APIToken at = db.Select<APIToken>()
                                .Where(a => a.id == apiToken)
                                .ToOne();

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