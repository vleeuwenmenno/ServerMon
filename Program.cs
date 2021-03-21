using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ServerMon.Constructors;
using ServerMon.Helpers;

namespace ServerMon
{
    public class Program
    {
        public static string programVersion = "v1.2.0";
        public static System.Threading.Thread loggingThread;
        public static Options options;
        public static IFreeSql db;


        public static void Main(string[] args)
        {
            options = Options.LoadOptions();
            db = Database.loadDatabase(options);

            loggingThread = new System.Threading.Thread(() => 
            {
                Timer logAgent = new Timer();            
                Timer cleanUpAgent = new Timer();
                
                logAgent.Interval = options.logging.interval * 1000;
                logAgent.Elapsed += logAgent_Elapsed;
                
                cleanUpAgent.Interval = 2 * 60 * 60 * 1000; // every 2 hours
                cleanUpAgent.Elapsed += cleanUpAgent_Elapsed;

                logAgent.Start();
                cleanUpAgent.Start();
            });
            loggingThread.Start();

            // Insert database tables
            db.Insert<APIToken>();
            db.Insert<SystemUsageLog>();

            processStartupArgs(args);

            CreateHostBuilder(args)
                .ConfigureAppConfiguration((hostingContext, config) =>
                {
                    var env = hostingContext.HostingEnvironment;
                    config.AddJsonFile("config/settings.json", optional: false, reloadOnChange: true);
                })
                .Build()
                .Run();
        }

        private static string shortHelpText = "ServerMon: usage: [ --serve | --api | --version ]";
        private static void printHelpText()
        {
            Console.WriteLine($"ServerMon {programVersion}");
            Console.WriteLine($"");
            Console.WriteLine(shortHelpText);
            Console.WriteLine($"");
            Console.WriteLine($"  Options:");
            Console.WriteLine($"    --help (-h)                 |   Show's this help text");
            Console.WriteLine($"    --serve (-s)                |   Start serving the API");
            Console.WriteLine($"    --api (-a)                  |   Modify/Remove/Add API tokens");
            Console.WriteLine($"        add (a)                 |   Add a new API token");
            Console.WriteLine($"        remove (r) TOKEN        |   Remove an API token");
            Console.WriteLine($"        extend (e) TOKEN DAYS   |   Extend an API token expiry date");
            Console.WriteLine($"    --version (-v)              |   Print the application version");
            Console.WriteLine($"");
        }

        private static void processStartupArgs(string[] args)
        {
            bool c = false;

            if (args.Count() > 0)
            {
                if (args[0] == "--api" || args[0] == "-a")
                {
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

                        db.Insert(token).ExecuteAffrows();

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

                        db.Delete<APIToken>(new APIToken() { id = args[2] }).ExecuteAffrows();
                    }
                    else if (args[1] == "extend" || args[1] == "e")
                    {
                        if (args.Count() < 4)
                        {
                            Console.WriteLine(shortHelpText);
                            return;
                        }

                        APIToken token = db.Select<APIToken>().Where(a => a.id == args[2]).ToOne();
                        token.expiry = token.expiry.AddDays(int.Parse(args[3]));
                        db.Update<APIToken>(new APIToken() {id = token.id })
                            .Set(a => a.expiry == token.expiry)
                            .ExecuteAffrows();
                    }
                    else if (args[1] == "list" || args[1] == "l")
                    {
                        List<APIToken> tokens = db.Select<APIToken>().ToList();

                        Console.WriteLine($"    |-----------------------------------------------------------------------|");
                        Console.WriteLine($"    |   API Secret                            |   Token expiry date         |");
                        Console.WriteLine($"    |-----------------------------------------------------------------------|");

                        foreach (APIToken token in tokens)
                            Console.WriteLine($"    |   {token.id}  |   {token.expiry.ToLongDateString()}   |");
                        
                        Console.WriteLine($"    |-----------------------------------------------------------------------|\n");
                    }
                }
                else if (args[0] == "--help" || args[0] == "-h")
                {
                    printHelpText();
                }
                else if (args[0] == "--version" || args[0] == "-v")
                {
                    Console.WriteLine($"ServerMon {programVersion}");
                }
                else if (args[0] == "--serve" || args[0] == "-s")
                {
                    Console.WriteLine($"ServerMon {programVersion}");
                    Console.WriteLine($"API begun serving HTTP on port {options.portHttp}");
                    Console.WriteLine($"API begun serving HTTPS on port {options.portHttps}");
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
        }

        private static void cleanUpAgent_Elapsed(object sender, ElapsedEventArgs e)
        {
            List<SystemUsageLog> logs = db.Select<SystemUsageLog>().ToList();
            List<List<decimal>> logsToReturn = new List<List<decimal>>();

            foreach (SystemUsageLog log in logs)
                if (log.timestamp < DateTime.UtcNow.AddDays(options.logging.lifeTime*-1))
                    db.Delete<SystemUsageLog>(log).ExecuteAffrows();
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

            db.Insert(log).ExecuteAffrows();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                    webBuilder.UseUrls($"https://*:{options.portHttps}", $"http://*:{options.portHttp}");
                });
    }
}
