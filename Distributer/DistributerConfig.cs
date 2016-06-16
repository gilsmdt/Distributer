using Distributer.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Distributer
{
    public class DistributerConfig : BaseConfig
    {
        public List<Machine> Machines { get; protected set; } = new List<Machine>();
        public string PsexecPath { get; protected set; }

        public DistributerConfig() { }

        public DistributerConfig(string configPath)
        {
            Load(configPath);
        }

        public override void Load(string configPath = null)
        {
            try
            {
                base.Load("distributer.json");

                // try to get psexec path from config
                PsexecPath = base.configData["psexecPath"]?.Value<string>();

                // if it wasn't specified try to find it in the app folder
                if (string.IsNullOrEmpty(PsexecPath))
                {
                    PsexecPath = Path.Combine(basePath, "psexec.exe");
                }

                if (!File.Exists(PsexecPath))
                    throw new Exception("cannot find psexecPath, please specify it in the config file");

                ParseJsonArray<Machine>(configData["machines"], Machines);

                if (Machines.Count == 0)
                    throw new Exception("0 machines were configured, please add one or more machines to the configuration file");
            }
            catch (Exception ex)
            {
                Console.Write("failed to initiate config: " + ex.Message);
                throw;
            }
        }
    }
}
