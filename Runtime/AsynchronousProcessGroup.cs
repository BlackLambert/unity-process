using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SBaier.Process
{
    public class AsynchronousProcessGroup : ProcessBase
    {
        private readonly IReadOnlyCollection<Process> _processes;
        private readonly List<Task> _tasks = new();
        
        public AsynchronousProcessGroup(IReadOnlyCollection<Process> processes)
        {
            _processes = processes;
        }
        
        protected override async Task RunInternal(CancellationToken token)
        {
            foreach (Process process in _processes)
            {
                _tasks.Add(process.Run(token));
            }

            await Task.WhenAll(_tasks);
            _tasks.Clear();
        }

        protected override float GetProgress()
        {
            return _processes.Sum(process => process.Progress.Value) / _processes.Count;
        }
    }
}