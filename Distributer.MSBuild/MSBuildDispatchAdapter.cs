using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributer.Data;
using Distributer.Contracts;
using System.ComponentModel.Composition;
using Distributer.MSBuild.Data;
using Distributer.Runners;

namespace Distributer.MSBuild
{
    public class MSBuildDispatchAdapter : DispatchAdapter
    {
        private BuildConfig buildConfig;
        private BuildRequest buildRequest;
        private BaseRunner runner;

        public MSBuildDispatchAdapter(BuildConfig buildConfig)
        {
            this.buildConfig = buildConfig;
        }

        public override void Dispatch(IRequest buildRequest, BaseRunner runner)
        {
            this.buildRequest = buildRequest as BuildRequest;
            this.runner = runner;

            GetSpecificVersion();
            Build();
        }

        private void Build()
        {
            var machine = buildRequest.Machine;
            var arguments = string.Format("\"{0}\" \"{1}\"", buildConfig.MSBuildPath, buildRequest.Project.BuildProjectPath);

            var result = runner.RunRemotly(buildRequest.Machine, arguments, buildRequest.Project.SourcePath);

            if (result != BaseRunner.Success)
                throw new Exception("failed running msbuild for " + buildRequest.Project.BuildProjectPath);
        }

        private void GetSpecificVersion()
        {
            var arguments = new StringBuilder(string.Format("\"{0}\" get /noprompt /recursive", buildConfig.TFPath));

            if (!string.IsNullOrEmpty(buildConfig.VersionNUmber))
            {
                arguments.AppendFormat(" /version:{0}", buildConfig.VersionNUmber);
            }

            if (buildRequest.Project.Override)
            {
                arguments.Append(" /overwrite");
            }

            var result = runner.RunRemotly(buildRequest.Machine, arguments.ToString(), buildRequest.Project.SourcePath);

            if (result != BaseRunner.Success)
                throw new Exception("get files from TFS failed");
        }
    }
}
