using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Distributer.Data;

namespace Distributer.Runners
{
    public class InvokeCommandRunner : BaseRunner
    {
        public override int RunRemotly(Machine machine, string command, string workingFolder = null)
        {
            throw new NotImplementedException();
        }
    }
}
