using Distributer.Contracts;
using Distributer.Data;
using Distributer.Runners;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Distributer.Contracts.DispatchAdapter;

namespace Distributer
{
    public class DistributionManager
    {
        public delegate void OutputHandler(string data, IRequest request);
        public event OutputHandler OnOutput;

        private BlockingCollection<int> processingQueue;
        private ConcurrentQueue<Machine> machinesQueue;
        private ConcurrentBag<Task> tasks = new ConcurrentBag<Task>();
        private IDispatcher dispatcher;
        private IRequest[] requests;
        private DistributerConfig config;
        private RunnerFactory runnerFactory;

        // will be used to indicate that we're still processing requests
        private volatile bool processing;

        public DistributionManager(IDispatcher dispatcher, DistributerConfig config)
        {
            this.config = config;
            this.dispatcher = dispatcher;
            this.runnerFactory = new RunnerFactory(config);
        }

        public async Task<bool> Distribute()
        {
            bool result = false;

            try
            {
                ValidateMachines();
                Init();
                result = await DistributeRequests();
            }
            catch (Exception ex)
            {
                NotifyOutput(string.Format("failed to process requests {0}\n{1}", ex.Message, ex.StackTrace));
            }

            return result;
        }

        private async Task<bool> DistributeRequests()
        {
            IRequest request;
            Machine machine;
            int requestIndex = 0;

            processing = true;

            while (requestIndex < requests.Length && processing)
            {
                request = requests[requestIndex];
                request.Id = requestIndex++;
                
                // we'll use the processing queue as our blocking mechanism
                processingQueue.Add(request.Id);

                // the block was released, we'll check if we're still processing
                if (!processing)
                {
                    break;
                }

                // get next available machine
                machinesQueue.TryDequeue(out machine);
                request.Machine = machine;

                Dispatch(request);
            }

            await Task.WhenAll(tasks);

            var result = tasks.All(task => task.IsCompleted);

            return result;
        }

        private void Init()
        {
            // we're initializing the processing queue with max capacity by the number of machines
            processingQueue = new BlockingCollection<int>(config.Machines.Count);
            machinesQueue = new ConcurrentQueue<Machine>();

            foreach (var machine in config.Machines)
            {
                machinesQueue.Enqueue(machine);
            }

            requests = dispatcher.GetRequests();
        }

        private void ValidateMachines()
        {
            foreach (var machine in config.Machines)
            {
                var runner = runnerFactory.GetRunner();
                var result = runner.RunRemotly(machine, "hostname");

                if (result != BaseRunner.Success)
                    throw new Exception("failed to validate remote execution using psexec on machine " + machine.IP);
            }
        }

        private void Dispatch(IRequest request)
        {
            // start a new task so we can run in parallel
            var task = Task.Factory.StartNew(() =>
            {
                var dispatchAdapter = dispatcher.GetDispatchAdapter(request);
                OnOutputHandler errorHandler = (data) => { NotifyOutput(data, request); };
                OnOutputHandler standardHandler = (data) => { NotifyOutput(data, request); };

                dispatchAdapter.OnErrorOutput += errorHandler;
                dispatchAdapter.OnStandardOutput += standardHandler;

                try
                {
                    var runner = runnerFactory.GetRunner();

                    dispatchAdapter.Dispatch(request, runner);
                }
                catch (Exception ex)
                {
                    NotifyOutput("processing request failed: " + ex.Message);
                    processing = false;
                    throw ex;
                }
                finally
                {
                    dispatchAdapter.OnErrorOutput -= errorHandler;
                    dispatchAdapter.OnStandardOutput -= standardHandler;
                }
            }).ContinueWith((previousTask) =>
            {
                processingQueue.Take();
                machinesQueue.Enqueue(request.Machine);
            });

            tasks.Add(task);
        }

        private void NotifyOutput(string data, IRequest request = null)
        {
            if (request != null)
            {
                Console.Write("{0} {1}: ", request.Id, request.Machine.IP);
            }

            Console.WriteLine(data);

            OnOutput?.Invoke(data, request);
        }
    }
}
