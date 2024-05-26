using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SBaier.Process
{
    public class AsynchronousProcessGroup : ProcessGroupBase
    {
        private readonly List<Task> _tasks = new();

        public AsynchronousProcessGroup(IReadOnlyCollection<Process> processes) : base(processes)
        {
            
        }

        protected override async Task RunInternal(CancellationToken token)
        {
            UpdateTotalAmount();
            RunInternalProcesses(token);

            while (!Done(token))
            {
                UpdateCurrentProcess();
                UpdateHandledAmount();
                await Task.Delay(_delayInMilliseconds, token);
            }

            Clean();
        }

        private void RunInternalProcesses(CancellationToken token)
        {
            foreach (Process process in _processes)
            {
                _tasks.Add(process.Run(token));
            }
        }

        private void Clean()
        {
            foreach (Task task in _tasks)
            {
                task.Dispose();
            }
            _tasks.Clear();
        }

        private void UpdateTotalAmount()
        {
            _totalProcessAmount.Value = _processes.Count;
        }

        private void UpdateHandledAmount()
        {
            int handledAmount = _processes.Count(process => process.Complete.Value);
            if (_handledProcessAmount.Value != handledAmount)
            {
                _handledProcessAmount.Value = handledAmount;
            }
        }

        private void UpdateCurrentProcess()
        {
            if (_currentProcess.Value == null || _currentProcess.Value.Complete.Value)
            {
                _currentProcess.Value = _processes.FirstOrDefault(process => !process.Complete.Value);
            }
        }

        protected override float GetProgress()
        {
            return _processes.Sum(process => process.Progress.Value) / _processes.Count;
        }

        private static bool TaskDone(Task task)
        {
            return task.IsCanceled || task.IsCompleted || task.IsFaulted;
        }

        private bool Done(CancellationToken token)
        {
            return _tasks.All(TaskDone) || token.IsCancellationRequested;
        }
    }
}