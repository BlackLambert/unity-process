using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SBaier.Process
{
    public class SynchronousProcessGroup : ProcessBase
    {
        private readonly IReadOnlyCollection<Process> _processes;
        
        public SynchronousProcessGroup(IReadOnlyCollection<Process> processes)
        {
            _processes = processes;
        }
        
        protected override async Task RunInternal(CancellationToken token)
        {
            foreach (Process process in _processes)
            {
                await process.Run(token);
            }
        }

        protected override float GetProgress()
        {
            return _processes.Sum(process => process.Progress.Value) / _processes.Count;
        }
    }
}