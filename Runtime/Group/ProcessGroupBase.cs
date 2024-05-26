using System.Collections.Generic;

namespace SBaier.Process
{
    public abstract class ProcessGroupBase : ProcessBase, ProcessGroup
    {
        public ReadonlyObservable<Process> CurrentProcess => _currentProcess;
        public ReadonlyObservable<int> HandledProcessesAmount => _handledProcessAmount;
        public ReadonlyObservable<int> TotalProcessesAmount => _totalProcessAmount;
        
        protected readonly IReadOnlyCollection<Process> _processes;
        protected readonly Observable<Process> _currentProcess = new();
        protected readonly Observable<int> _handledProcessAmount = new();
        protected readonly Observable<int> _totalProcessAmount = new();
        
        protected ProcessGroupBase(IReadOnlyCollection<Process> processes)
        {
            _processes = processes;
        }

        public override void Dispose()
        {
            base.Dispose();
            foreach (Process process in _processes)
            {
                process.Dispose();
            }
        }
    }
}