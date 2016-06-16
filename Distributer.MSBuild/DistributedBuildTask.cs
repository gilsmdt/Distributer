using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Build.Framework;
using System.Threading.Tasks;

namespace Distributer.MSBuild
{
    public class DistributedBuildTask : Microsoft.Build.Utilities.Task
    {
        [Required]
        public string VersionNumber { get; set; }

        [Required]
        public string DistributerConfigPath { get; set; }

        [Required]
        public string BuildConfigPath { get; set; }

        private DistributionManager distributionManager;

        public DistributedBuildTask() : base()
        {
            var distributerConfig = new DistributerConfig(DistributerConfigPath);
            var buildConfig = new BuildConfig(BuildConfigPath);
            var msbuildDistributerPlugin = new MSBuildDispatcher(buildConfig);

            distributionManager = new DistributionManager(msbuildDistributerPlugin, distributerConfig);
        }

        public override bool Execute()
        {
            var result = this.distributionManager.Distribute().Result;

            return result;
        }
    }
}
