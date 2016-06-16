using Distributer.Contracts;
using Distributer.Data;
using Distributer.MSBuild.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributer.MSBuild
{
    public class MSBuildDispatcher : IDispatcher
    {
        private BuildConfig buildConfig;

        public MSBuildDispatcher(BuildConfig buildConfig)
        {
            if (buildConfig == null)
                throw new ArgumentNullException("buildConfig");

            this.buildConfig = buildConfig;
        }

        public DispatchAdapter GetDispatchAdapter(IRequest request)
        {
            return new MSBuildDispatchAdapter(buildConfig);
        }

        public IRequest[] GetRequests()
        {
            BuildRequest[] requests = this.buildConfig.Projects.Select(item => new BuildRequest() { Project = item }).ToArray();

            return requests;
        }
    }
}
