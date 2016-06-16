using Distributer.Contracts;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Distributer
{
    internal class PluginsManager
    {
        private DistributerConfig config;

        [ImportMany()]
        IEnumerable<Lazy<IDistributerPlugin, PluginMetaData>> distributerPlugins;

        private CompositionContainer container;

        public PluginsManager(DistributerConfig config)
        {
            this.config = config;

            var catalog = new DirectoryCatalog(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location));
            container = new CompositionContainer(catalog);

            container.ComposeParts(this);
        }

        public IDistributerPlugin GetDistributerPlugin()
        {
            if (config.DistributerPlugin == null)
                throw new Exception("distributerPlugin has no value, please specify it in the config file");

            var plugin = GetPlugin<IDistributerPlugin>(config.DistributerPlugin, distributerPlugins);

            return plugin;
        }

        private T GetPlugin<T>(string pluginName, IEnumerable<Lazy<T, PluginMetaData>> plugins)
        {
            T instance = default(T);

            if (plugins != null)
            {
                foreach (Lazy<T, PluginMetaData> plugin in plugins)
                {
                    if (plugin.Metadata.Name == pluginName)
                    {
                        instance = plugin.Value;
                        break;
                    }
                }
            }

            if (instance == null)
                throw new Exception("cannot find plugins with name " + pluginName + " and type " + typeof(T).FullName);

            return instance;
        }
    }
}
