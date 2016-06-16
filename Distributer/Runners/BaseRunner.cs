using Distributer.Data;
using System.Diagnostics;
using System.Text;
using System.Threading;

namespace Distributer.Runners
{
    public abstract class BaseRunner
    {
        public static readonly int Error = -1;
        public static readonly int Success = 0;

        //TODO: move to config
        protected int RunTimeout { get; set; } = 1000 * 600;
        protected int ConnectTimeout { get; set; } = 1000 * 30;

        public delegate void OnOutputHandler(string output);
        public event OnOutputHandler OnStandardOutput;
        public event OnOutputHandler OnErrorOutput;

        public abstract int RunRemotly(Machine machine, string command, string workingFolder = null);

        public int Run(string filename, string arguments)
        {
            using (Process process = new Process())
            {
                process.StartInfo.FileName = filename;
                process.StartInfo.Arguments = arguments;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                StringBuilder output = new StringBuilder();
                StringBuilder error = new StringBuilder();

                using (AutoResetEvent outputWaitHandle = new AutoResetEvent(false))
                using (AutoResetEvent errorWaitHandle = new AutoResetEvent(false))
                {
                    process.OutputDataReceived += (sender, e) => {
                        if (e.Data == null)
                        {
                            outputWaitHandle.Set();
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(e.Data))
                            {
                                WriteStandardOutput(e.Data);
                            }
                        }
                    };

                    process.ErrorDataReceived += (sender, e) =>
                    {
                        if (e.Data == null)
                        {
                            errorWaitHandle.Set();
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(e.Data))
                            {
                                WriteErrorOutput(e.Data);
                            }
                        }
                    };

                    process.Start();
                    process.BeginOutputReadLine();
                    process.BeginErrorReadLine();

                    if (process.WaitForExit(RunTimeout) &&
                        outputWaitHandle.WaitOne(RunTimeout) &&
                        errorWaitHandle.WaitOne(RunTimeout))
                    {
                        return process.ExitCode;
                    }
                    else
                    {
                        // timeout
                        return Error;
                    }
                }
            }
        }

        protected void WriteStandardOutput(string data)
        {
            OnStandardOutput?.Invoke(data);
        }

        protected void WriteErrorOutput(string data)
        {
            OnErrorOutput?.Invoke(data);
        }
    }
}
