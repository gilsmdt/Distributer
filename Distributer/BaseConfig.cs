using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributer
{
    public class BaseConfig
    {
        protected static readonly string basePath = AppDomain.CurrentDomain.BaseDirectory;
        protected readonly string configFile;

        protected JObject configData;

        public virtual void Load(string configFile)
        {
            if (!File.Exists(configFile))
                throw new Exception("cannot find config file " + configFile);

            var jsonData = File.ReadAllText(configFile);

            configData = JObject.Parse(jsonData);
        }

        protected void ParseJsonArray<T>(JToken token, List<T> collection)
        {
            var tokens = token.Children().ToList();

            foreach (var childToken in tokens)
            {
                var item = JsonConvert.DeserializeObject<T>(childToken.ToString());

                collection.Add(item);
            }
        }
    }
}
