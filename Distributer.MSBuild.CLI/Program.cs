using Commons.GetOptions;
using Distributer.Data;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributer.MSBuild.CLI
{     
    class Program
    {
        static void Main(string[] args)
        {
            var msbuildOptions = new MSBuildOptions(args);

            if (msbuildOptions.Valid)
            {
                var distributerConfig = new DistributerConfig(msbuildOptions.DistributerConfigPath);
                var buildConfig = new BuildConfig(msbuildOptions.BuildConfigPath)
                {
                    VersionNUmber = msbuildOptions.VersionNumber
                };
                var msbuildDispatcher = new MSBuildDispatcher(buildConfig);
                var distributionManager = new DistributionManager(msbuildDispatcher, distributerConfig);

                Task.Run(async () =>
                {
                    await distributionManager.Distribute();
                }).Wait();
            }
        }
    }
}