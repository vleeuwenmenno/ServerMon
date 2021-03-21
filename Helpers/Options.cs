using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace ServerMon.Helpers
{
    public struct Logging
    {
        /// <summary>
        /// Interval to create logs
        /// </summary>
        /// <value></value>
        public int interval {get;set;}

        /// <summary>
        /// Life time of a log in days
        /// </summary>
        /// <value></value>
        public int lifeTime {get;set;}
    }

    public struct Database
    {
        public string dbType {get;set;}
        public string sqliteFile {get;set;}

        public string host {get;set;}
        public string user {get;set;}
        public string pass {get;set;}
        public string dbName {get;set;}
        public int port {get;set;}

        public static IFreeSql loadDatabase(Options options)
        {
            if (options.database.dbType.ToLower() == "mysql")
            {
                bool missingParams = false;


                if (missingParams)
                {
                    Console.WriteLine("One or more of the following config keys are missing. (host, port, user, password and/or database)");
                    Environment.Exit(-102);
                }

                return new FreeSql.FreeSqlBuilder()
                    .UseConnectionString(FreeSql.DataType.MySql, $@"Data Source={options.database.host};Port={options.database.port};User ID={options.database.user};Password={options.database.pass};Initial Catalog={options.database.dbName};Charset=utf8;SslMode=none;Max pool size=20")
                    .UseAutoSyncStructure(options.isDev)
                    .Build();
            }
            else if (options.database.dbType.ToLower() == "sqlite")
            {
                if (!string.IsNullOrEmpty(options.database.sqliteFile))
                {
                    Console.WriteLine("Missing config key for database fileName, can't continue  without this!");
                    Environment.Exit(-101);                    
                }

                return new FreeSql.FreeSqlBuilder()
                    .UseConnectionString(FreeSql.DataType.Sqlite, $@"Data Source=|DataDirectory|{options.database.sqliteFile};Pooling=true;Max Pool Size=10")
                    .UseAutoSyncStructure(options.isDev)
                    .Build();
            }
            else
                return null;
        }
    }

    public class Options
    {
        #region Application
        public int portHttp {get;set;}
        public int portHttps {get;set;}
        
        public bool isDev {get;set;}

        public Logging logging {get;set;}
        public Database database {get;set;}
        #endregion
        
        private string configLocation {get; set;}

        public static Options LoadOptions()
        {
            Options options = new Options();
            if (string.IsNullOrEmpty(options.configLocation) || !File.Exists(options.configLocation))
                options.configLocation = Environment.CurrentDirectory +  "/config";

            if (!File.Exists($"{options.configLocation}/config.json"))
            {
                Directory.CreateDirectory($"{options.configLocation}");
                File.WriteAllText($"{options.configLocation}/config.json", JsonSerializer.Serialize(options, new JsonSerializerOptions() { WriteIndented = true }));
            }

            return JsonSerializer.Deserialize<Options>(File.ReadAllText($"{options.configLocation}/config.json"));
        }

        public static Options LoadOptions(string configLocation)
        {
            Options opt = new Options();
            opt.configLocation = configLocation;
            return opt;
        }

        public void SaveOptions()
        {
            File.WriteAllText($"{configLocation}/config.json", JsonSerializer.Serialize(this, new JsonSerializerOptions() { WriteIndented = true }));
        }
    }
}