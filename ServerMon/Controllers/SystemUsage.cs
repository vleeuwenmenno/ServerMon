using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System.Collections.Generic;
using ServerMon.Constructors;
using System.Linq;
using System;
using Auth;

namespace ServerMon.Controllers
{
    [ApiController]
    public class SystemUsageController : ControllerBase
    {
        readonly ILogger<SystemUsageController> _log;
        
        public SystemUsageController(ILogger<SystemUsageController> log)
        {
            _log = log;
        }
        
        [HttpGet("current-usage")] 
        public ActionResult<Dictionary<string, object>> Get([FromHeader] string Authorization)
        {
            _log.LogInformation($"[{Request.HttpContext.Connection.RemoteIpAddress.ToString()}] Requested [GET] /current-usage");

            if (Authentication.VerifyAPIAccess(Authorization))
            {
                SystemUsage systemUsage = SystemUsage.profile();
                return Ok(systemUsage);
            }
            else
                return Unauthorized();
        }
        
        [HttpGet("current-usage/cpu")] 
        public ActionResult<Dictionary<string, object>> GetCpu([FromHeader] string Authorization)
        {
            _log.LogInformation($"[{Request.HttpContext.Connection.RemoteIpAddress.ToString()}] Requested [GET] /current-usage");

            if (Authentication.VerifyAPIAccess(Authorization))
            {
                SystemUsage systemUsage = SystemUsage.profile();
                return Ok(systemUsage.cpu);
            }
            else
                return Unauthorized();
        }
        
        [HttpGet("current-usage/memory")] 
        public ActionResult<Dictionary<string, object>> GetMem([FromHeader] string Authorization)
        {
            _log.LogInformation($"[{Request.HttpContext.Connection.RemoteIpAddress.ToString()}] Requested [GET] /current-usage");

            if (Authentication.VerifyAPIAccess(Authorization))
            {
                SystemUsage systemUsage = SystemUsage.profile();
                return Ok(systemUsage.memory);
            }
            else
                return Unauthorized();
        }
        
        [HttpGet("current-usage/swap")] 
        public ActionResult<Dictionary<string, object>> GetSwap([FromHeader] string Authorization)
        {
            _log.LogInformation($"[{Request.HttpContext.Connection.RemoteIpAddress.ToString()}] Requested [GET] /current-usage");

            if (Authentication.VerifyAPIAccess(Authorization))
            {
                SystemUsage systemUsage = SystemUsage.profile();
                return Ok(systemUsage.swap);
            }
            else
                return Unauthorized();
        }
        
        [HttpGet("usage/{hours}")] 
        public ActionResult GetHistory(int hours, [FromHeader] string Authorization)
        {
            _log.LogInformation($"[{Request.HttpContext.Connection.RemoteIpAddress.ToString()}] Requested [GET] /current-usage");

            if (Authentication.VerifyAPIAccess(Authorization))
            {
                long earliestDataPoint = ((long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds)-(hours * 60 * 60);
                List<SystemUsageLog> logs = Authentication.db.Table<SystemUsageLog>().ToList();
                List<List<decimal>> logsToReturn = new List<List<decimal>>();

                foreach (SystemUsageLog log in logs)
                {
                    if (((log.timestamp).Subtract(new DateTime(1970, 1, 1))).TotalSeconds > earliestDataPoint)
                    {
                        List<decimal> l = new List<decimal>();

                        l.Add(log.user);
                        l.Add(log.system);
                        l.Add(log.wait);
                        l.Add(log.idle);

                        l.Add(log.totalMemory);
                        l.Add(log.usedMemory);
                        l.Add(log.freeMemory);

                        l.Add(log.totalSwap);
                        l.Add(log.usedSwap);
                        l.Add(log.freeSwap);

                        logsToReturn.Add(l);
                    }
                }
                
                return Ok(logsToReturn);
            }
            else
                return Unauthorized();
        }
        
        [HttpGet("usage/{hours}/swap/{type}")] 
        public ActionResult GetSwapHistory(int hours, string type, [FromHeader] string Authorization)
        {
            _log.LogInformation($"[{Request.HttpContext.Connection.RemoteIpAddress.ToString()}] Requested [GET] /current-usage");

            if (Authentication.VerifyAPIAccess(Authorization))
            {
                long earliestDataPoint = ((long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds)-(hours * 60 * 60);
                List<SystemUsageLog> logs = Authentication.db.Table<SystemUsageLog>().ToList();
                List<decimal> logsToReturn = new List<decimal>();

                foreach (SystemUsageLog log in logs)
                {
                    if (((log.timestamp).Subtract(new DateTime(1970, 1, 1))).TotalSeconds > earliestDataPoint)
                    {
                        if (type == "free")
                            logsToReturn.Add(log.freeSwap);

                        if (type == "total")
                            logsToReturn.Add(log.totalSwap);

                        if (type == "used")
                            logsToReturn.Add(log.usedSwap);
                    }
                }
                
                return Ok(logsToReturn);
            }
            else
                return Unauthorized();
        }
        
        [HttpGet("usage/{hours}/memory/{type}")] 
        public ActionResult GetMemHistory(int hours, string type, [FromHeader] string Authorization)
        {
            _log.LogInformation($"[{Request.HttpContext.Connection.RemoteIpAddress.ToString()}] Requested [GET] /current-usage");

            if (Authentication.VerifyAPIAccess(Authorization))
            {
                long earliestDataPoint = ((long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds)-(hours * 60 * 60);
                List<SystemUsageLog> logs = Authentication.db.Table<SystemUsageLog>().ToList();
                List<decimal> logsToReturn = new List<decimal>();

                foreach (SystemUsageLog log in logs)
                {
                    if (((log.timestamp).Subtract(new DateTime(1970, 1, 1))).TotalSeconds > earliestDataPoint)
                    {
                        if (type == "free")
                            logsToReturn.Add(log.freeMemory);

                        if (type == "total")
                            logsToReturn.Add(log.totalMemory);

                        if (type == "used")
                            logsToReturn.Add(log.usedMemory);
                    }
                }
                
                return Ok(logsToReturn);
            }
            else
                return Unauthorized();
        }
        
        [HttpGet("usage/{hours}/cpu/{type}")] 
        public ActionResult GetCpuHistory(int hours, string type, [FromHeader] string Authorization)
        {
            _log.LogInformation($"[{Request.HttpContext.Connection.RemoteIpAddress.ToString()}] Requested [GET] /current-usage");

            if (Authentication.VerifyAPIAccess(Authorization))
            {
                long earliestDataPoint = ((long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds)-(hours * 60 * 60);
                List<SystemUsageLog> logs = Authentication.db.Table<SystemUsageLog>().ToList();
                List<decimal> logsToReturn = new List<decimal>();

                foreach (SystemUsageLog log in logs)
                {
                    if (((log.timestamp).Subtract(new DateTime(1970, 1, 1))).TotalSeconds > earliestDataPoint)
                    {
                        if (type == "used")
                            logsToReturn.Add(log.user+log.system+log.wait);

                        if (type == "user")
                            logsToReturn.Add(log.user);

                        if (type == "system")
                            logsToReturn.Add(log.system);

                        if (type == "wait")
                            logsToReturn.Add(log.wait);

                        if (type == "idle")
                            logsToReturn.Add(log.idle);
                    }
                }
                
                return Ok(logsToReturn);
            }
            else
                return Unauthorized();
        }
    }
}
