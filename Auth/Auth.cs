using System;
using System.Collections.Generic;
using System.Linq;
using SQLite;

namespace Auth
{
    public class Authentication
    {
        public static Options options = new Options();
        public static SQLiteConnection db;
        
        public static bool VerifyAPIAccess(string apiToken)
        {
            if (string.IsNullOrEmpty(apiToken) || !apiToken.StartsWith("Bearer"))
                return false; 

            apiToken = apiToken.Replace("Bearer", "").Trim();

            List<APIToken> at = Authentication.db.Query<APIToken>($"SELECT * FROM APIToken WHERE id = '{apiToken}';");
            APIToken atoken = at.Count > 0 ? at.First() : null;

            if (atoken == null)
                return false;
            else
            {
                //Check if the session is expired;
                if (atoken.expiry < DateTime.UtcNow)
                    return false;
                
                else
                    return true;
            }
        }
    }
}