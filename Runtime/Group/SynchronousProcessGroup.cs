using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SBaier.Process
{
    public class SynchronousProcessGroup : ProcessGroupBase
    {
        public SynchronousProcessGroup(IReadOnlyCollection<Process> processes) : base(processes)
        {
            
        }

        protected override async Task RunInternal(CancellationToken token)
        {
            _totalProcessAmount.Value = _processes.Count;
            _handledProcessAmount.Value = 0;
            
            foreach (Process process in _processes)
            {
                _currentProcess.Value = process;
                await process.Run(token);
                _handledProcessAmount.Value++;
            }
        }

        protected override float GetProgress()
        {
            return _processes.Sum(process => process.Progress.Value) / _processes.Count;
        }
    }
}