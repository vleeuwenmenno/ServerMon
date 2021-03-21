using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

using System.Collections.Generic;
using ServerMon.Constructors;
using ServerMon.Helpers;
using ServerMon.Helpers.Authorization;
using System.ComponentModel;

namespace ServerMon.Controllers
{
    [Route("v1")]
    [ApiController]
    public class SystemUsageController : ControllerBase
    {
        readonly ILogger<SystemUsageController> _log;
        readonly IFreeSql _db;
        readonly Options _options;
        
        public SystemUsageController(ILogger<SystemUsageController> log, IFreeSql fsql, Options options)
        {
            _log = log;
            _db = fsql;
            _options = options;
        }
        
        [HttpGet("current-usage")]
        public ActionResult<Dictionary<string, object>> Get()
        {
            _log.LogInformation($"[{Request.HttpContext.Connection.RemoteIpAddress.ToString()}] Requested [GET] /current-usage");

            SystemUsage systemUsage = SystemUsage.profile();
            return Ok(systemUsage);
        }

        [HttpGet("current-usage/cpu")] 
        public ActionResult<Dictionary<string, object>> GetCpu()
        {
            SystemUsage systemUsage = SystemUsage.profile();
            return Ok(systemUsage.cpu);
        }
        
        [HttpGet("current-usage/memory")] 
        public ActionResult<Dictionary<string, object>> GetMem()
        {
            SystemUsage systemUsage = SystemUsage.profile();
            return Ok(systemUsage.memory);
        }
        
        [HttpGet("current-usage/swap")] 
        public ActionResult<Dictionary<string, object>> GetSwap()
        {
            SystemUsage systemUsage = SystemUsage.profile();
            return Ok(systemUsage.swap);
        }
        
        [HttpGet("usage/{hours}")] 
        public ActionResult GetHistory(int hours)
        {
            long earliestDataPoint = ((long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds)-(hours * 60 * 60);
            List<SystemUsageLog> logs = _db.Select<SystemUsageLog>().ToList();
            List<List<decimal>> logsToReturn = new List<List<decimal>>();

            foreach (SystemUsageLog log in logs)
            {
                if (((log.timestamp).Subtract(new DateTime(1970, 1, 1))).TotalSeconds > earliestDataPoint)
                {
                    List<decimal> l = new List<decimal>();

                    l.Add(log.user);
                    l.Add(log.nice);
                    l.Add(log.system);
                    l.Add(log.iowait);
                    l.Add(log.steal);
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
        
        [HttpGet("usage/{hours}/swap/{type}")] 
        public ActionResult GetSwapHistory(int hours, string type)
        {
            long earliestDataPoint = ((long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds)-(hours * 60 * 60);
            List<SystemUsageLog> logs = _db.Select<SystemUsageLog>().ToList();
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
        
        [HttpGet("usage/{hours}/memory/{type}")] 
        public ActionResult GetMemHistory(int hours, string type)
        {
            long earliestDataPoint = ((long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds)-(hours * 60 * 60);
            List<SystemUsageLog> logs = _db.Select<SystemUsageLog>().ToList();
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
        
        [HttpGet("usage/{hours}/cpu/{type}")] 
        public ActionResult GetCpuHistory(int hours, string type)
        {
            long earliestDataPoint = ((long)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds)-(hours * 60 * 60);
            List<SystemUsageLog> logs = _db.Select<SystemUsageLog>().ToList();
            List<decimal> logsToReturn = new List<decimal>();

            foreach (SystemUsageLog log in logs)
            {
                if (((log.timestamp).Subtract(new DateTime(1970, 1, 1))).TotalSeconds > earliestDataPoint)
                {
                    if (type == "user")
                        logsToReturn.Add(log.user);
                        
                    if (type == "nice")
                        logsToReturn.Add(log.nice);
                        
                    if (type == "system")
                        logsToReturn.Add(log.system);
                        
                    if (type == "iowait")
                        logsToReturn.Add(log.iowait);
                        
                    if (type == "steal")
                        logsToReturn.Add(log.steal);
                        
                    if (type == "idle")
                        logsToReturn.Add(log.idle);

                    if (type == "idle-invert")
                        logsToReturn.Add((log.idle-100)*-1);
                }
            }
            
            return Ok(logsToReturn);
        }
    }
}
