using System;
using System.Linq;

using System.Collections.Generic;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;


using SQLite;
using ServerMon.Constructors;
using System.Timers;
using Auth;

namespace ServerMon
{
    public class Program
    {
        public static string shortHelpText = "ServerMon: usage: [ --serve | --api | --version ]";
        public static string version = "v1.0.3";

        public static void Main(string[] args)
        {
            bool c = false;

            // Check if we have a database
            Authentication.options.LoadOptions();

            if (args.Count() > 0)
            {
                if (args[0] == "--api" || args[0] == "-a")
                {
                    Authentication.db = new SQLiteConnection(Environment.CurrentDirectory + "/database.sqlite");
                    
                    if (args.Count() < 2)
                    {
                        Console.WriteLine(shortHelpText);
                        return;
                    }

                    if (args[1] == "add" || args[1] == "a")
                    {
                        APIToken token = new APIToken();

                        token.id = Guid.NewGuid().ToString();
                        token.expiry = DateTime.UtcNow.AddDays(365 * 2);

                        Authentication.db.Insert(token);

                        Console.WriteLine($"    |-----------------------------------------------------------------------|");
                        Console.WriteLine($"    |   API Secret                            |   Token expiry date         |");
                        Console.WriteLine($"    |-----------------------------------------------------------------------|");
                        Console.WriteLine($"    |   {token.id}  |   {token.expiry.ToLongDateString()}   |");
                        Console.WriteLine($"    |-----------------------------------------------------------------------|\n");
                    }
                    else if (args[1] == "remove" || args[1] == "r")
                    {
                        if (args.Count() < 3)
                        {
                            Console.WriteLine(shortHelpText);
                            return;
                        }

                        Authentication.db.Query<APIToken>($"DELETE FROM APIToken WHERE id={args[2]}");
                    }
                    else if (args[1] == "extend" || args[1] == "e")
                    {
                        if (args.Count() < 4)
                        {
                            Console.WriteLine(shortHelpText);
                            return;
                        }

                        APIToken token = Authentication.db.Query<APIToken>($"SELECT * FROM APIToken WHERE id={args[2]}").Last();
                        token.expiry.AddDays(int.Parse(args[3]));
                        Authentication.db.Update(token);
                    }
                    else if (args[1] == "list" || args[1] == "l")
                    {
                        List<APIToken> tokens = Authentication.db.Query<APIToken>("SELECT * FROM APIToken WHERE 1;");

                        Console.WriteLine($"    |-----------------------------------------------------------------------|");
                        Console.WriteLine($"    |   API Secret                            |   Token expiry date         |");
                        Console.WriteLine($"    |-----------------------------------------------------------------------|");
                        foreach (APIToken token in tokens)
                        {
                            Console.WriteLine($"    |   {token.id}  |   {token.expiry.ToLongDateString()}   |");
                        }
                        Console.WriteLine($"    |-----------------------------------------------------------------------|\n");
                    }
                }
                else if (args[0] == "--help" || args[0] == "-h")
                {
                    printHelpText();
                }
                else if (args[0] == "--version" || args[0] == "-v")
                {
                    Console.WriteLine($"ServerMon {version}");
                }
                else if (args[0] == "--serve" || args[0] == "-s")
                {
                    Console.WriteLine($"ServerMon {version}");
                    Console.WriteLine($"API begun serving on port {Authentication.options.apiPort}");
                    Console.WriteLine($"Logging to {Environment.CurrentDirectory}/logs");

                    c = true;
                }

                if (!c)
                    Environment.Exit(0);
            }
            else
            {
                Console.WriteLine(shortHelpText);
                Environment.Exit(0);
            }

            Authentication.db = new SQLiteConnection(Environment.CurrentDirectory + "/database.sqlite");
            
            Authentication.db.CreateTable<APIToken>();
            Authentication.db.CreateTable<SystemUsageLog>();

            Timer logAgent = new Timer();            
            Timer cleanUpAgent = new Timer();
            
            logAgent.Interval = Authentication.options.interval * 1000;
            logAgent.Elapsed += logAgent_Elapsed;
            
            cleanUpAgent.Interval = 2 * 60 * 60 * 1000; // every 2 hours
            cleanUpAgent.Elapsed += cleanUpAgent_Elapsed;

            logAgent.Start();
            cleanUpAgent.Start();

            CreateHostBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;

                    config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

                })
                .Build()
                .Run();
        }

        private static void cleanUpAgent_Elapsed(object sender, ElapsedEventArgs e)
        {
            List<SystemUsageLog> logs = Authentication.db.Table<SystemUsageLog>().ToList();
            List<List<decimal>> logsToReturn = new List<List<decimal>>();

            foreach (SystemUsageLog log in logs)
                if (log.timestamp < DateTime.UtcNow.AddDays(Authentication.options.logLifeTime*-1))
                    Authentication.db.Delete(log);
        }

        private static void logAgent_Elapsed(object sender, ElapsedEventArgs e)
        {
            SystemUsage systemUsage = SystemUsage.profile();
            SystemUsageLog log = new SystemUsageLog()
            {
                user = systemUsage.cpu.user,
                nice = systemUsage.cpu.nice,
                system = systemUsage.cpu.system,
                iowait = systemUsage.cpu.iowait,
                steal = systemUsage.cpu.steal,
                idle = systemUsage.cpu.idle,

                totalMemory = systemUsage.memory.totalMemory,
                usedMemory = systemUsage.memory.usedMemory,
                freeMemory = systemUsage.memory.freeMemory,

                totalSwap = systemUsage.swap.totalSwap,
                usedSwap = systemUsage.swap.usedSwap,
                freeSwap = systemUsage.swap.freeSwap,

                timestamp = DateTime.UtcNow
            };

            Authentication.db.Insert(log);
        }

        private static void printHelpText()
        {
            Console.WriteLine($"ServerMon {version}");
            Console.WriteLine($"");
            Console.WriteLine(shortHelpText);
            Console.WriteLine($"");
            Console.WriteLine($"  Options:");
            Console.WriteLine($"    --help (-h)                 |   Show's this help text");
            Console.WriteLine($"    --serve (-s)                |   Start serving the API");
            Console.WriteLine($"    --api (-a)                  |   Modify/Remove/Add API tokens");
            Console.WriteLine($"        --add (a)               |   Add a new API token");
            Console.WriteLine($"        --remove (r) TOKEN      |   Remove an API token");
            Console.WriteLine($"        --extend (e) TOKEN DAYS |   Extend an API token expiry date");
            Console.WriteLine($"    --version (-v)              |   Print the application version");
            Console.WriteLine($"");
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls(new string[] { $"http://0.0.0.0:{Authentication.options.apiPort}/" });
                });
    }
}