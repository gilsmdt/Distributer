using Distributer.Data;
using Distributer.Runners;

namespace Distributer.Contracts
{
    public abstract class DispatchAdapter
    {
        public delegate void OnOutputHandler(string output);
        public event OnOutputHandler OnStandardOutput;
        public event OnOutputHandler OnErrorOutput;

        public abstract void Dispatch(IRequest distributableRequest, BaseRunner runner);
    }
}
