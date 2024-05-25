using System;
using System.Threading;
using System.Threading.Tasks;

namespace SBaier.Process
{
    public class TaskProcess : ProcessBase
    {
        private Func<Task> _taskCreationFunction;
        private Task _task;
        
        public TaskProcess(Func<Task> taskCreationFunction)
        {
            _taskCreationFunction = taskCreationFunction;
        }

        protected override async Task RunInternal(CancellationToken token)
        {
            _task = _taskCreationFunction();
            await _task;
        }

        protected override float GetProgress()
        {
            return _task is { IsCompleted: true } ? 1 : 0;
        }
    }
}