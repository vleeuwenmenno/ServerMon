using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace Auth
{

    public struct Options
    {
        #region Application
        public int apiPort {get;set;}
        public int interval {get;set;}
        public int logLifeTime {get;set;}
        public bool debug {get;set;}

        public Dictionary<string, string> database {get;set;}
        #endregion

        
        internal static string confLocation = Environment.CurrentDirectory +  "/config";

        public void LoadOptions()
        {
            if (!File.Exists($"{confLocation}/config.json"))
            {
                Directory.CreateDirectory($"{confLocation}");
                File.WriteAllText($"{confLocation}/config.json", JsonSerializer.Serialize(this, new JsonSerializerOptions() { WriteIndented = true }));
            }
            else
            {
                this = JsonSerializer.Deserialize<Options>(File.ReadAllText($"{confLocation}/config.json"));
            }
        }

        public string GetJson()
        {
            return JsonSerializer.Serialize(this, new JsonSerializerOptions() { WriteIndented = true });
        }

        public void SaveOptions()
        {
            File.WriteAllText($"{confLocation}/config.json", JsonSerializer.Serialize(this, new JsonSerializerOptions() { WriteIndented = true }));
        }
    }
}