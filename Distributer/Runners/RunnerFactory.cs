using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Distributer.Runners
{
    public class RunnerFactory
    {
        private DistributerConfig config;

        public RunnerFactory(DistributerConfig config)
        {
            this.config = config;
        }

        public BaseRunner GetRunner()
        {
            return new PSExecRunner(config.PsexecPath);
        }
    }
}
