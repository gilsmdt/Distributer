using Distributer.MSBuild.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace Distributer.MSBuild
{
    public class BuildConfig : BaseConfig
    {
        public List<Project> Projects { get; } = new List<Project>();
        public string MSBuildPath { get; set; }
        public string TFPath { get; set; }
        public string VersionNUmber { get; set; }

        public BuildConfig() { }

        public BuildConfig(string configPath)
        {
            Load(configPath);
        }

        public void Load(string configPath = null, string versionNumber = null) 
        {
            base.Load(configPath ?? "build.json");

            this.VersionNUmber = versionNumber;
            
            // try to get dispatch adapter factory name from config file
            MSBuildPath = base.configData["msbuildPath"]?.Value<string>();

            if (MSBuildPath == null)
                throw new Exception("msbuildPath has no value, please specify it in the config file");

            // try to get dispatch adapter factory name from config file
            TFPath = base.configData["tfPath"]?.Value<string>();

            if (TFPath == null)
                throw new Exception("tfPath has no value, please specify it in the config file");

            ParseJsonArray<Project>(configData["projects"], Projects);

            if (Projects.Count == 0)
                throw new Exception("0 machines were projects, please add one or more projects to the configuration file");
        }
    }
}
