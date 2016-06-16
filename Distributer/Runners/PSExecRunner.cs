using Distributer.Data;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Distributer.Runners
{
    public class PSExecRunner : BaseRunner
    {
        public string psexecPath { get; set; }

        public PSExecRunner(string psexecPath)
        {
            this.psexecPath = psexecPath;
        }

        public override int RunRemotly(Machine machine, string command, string workingFolder = null)
        {
            var arguments = new StringBuilder(string.Format(@"\\{0} -accepteula -n {1}", machine.IP, base.ConnectTimeout));

            if (!string.IsNullOrEmpty(workingFolder))
            {
                arguments.AppendFormat(" -w \"{0}\"", workingFolder);
            }

            if (!string.IsNullOrEmpty(machine.Username) && !string.IsNullOrEmpty(machine.Password))
            {
                arguments.AppendFormat(" -e -u {0} -p {1}", machine.Username, machine.Password);
            }

            arguments.AppendFormat(" {0}", command);

            var result = Run(psexecPath, arguments.ToString());

            return result;
        }
    }
}
