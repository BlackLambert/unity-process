using System.Threading;
using System.Threading.Tasks;
using SBaier.DI;
using UnityEngine;

namespace SBaier.Process
{
    public abstract class ProcessStarterBase : MonoBehaviour, Injectable, Initializable, Cleanable
    {
        private ProcessQueue _queue;
        private Observable<Process> _currentProcess;
        private CancellationTokenSource _cancellationTokenSource;
        private bool _stopped = false;

        public virtual void Inject(Resolver resolver)
        {
            _queue = resolver.Resolve<ProcessQueue>();
            _currentProcess = resolver.Resolve<Observable<Process>>();
        }

        public async void Initialize()
        {
            _queue.OnEnqueue += OnProcessEnqueued;
            await TryStartNextProcess(immediately: false);
        }

        public void Clean()
        {
            _queue.OnEnqueue -= OnProcessEnqueued;
            TryStopCurrentProcess();
        }

        private async Task TryStartNextProcess(bool immediately)
        {
            if (!CanStartNextProcess())
            {
                return;
            }
            
            Process process = _queue.Dequeue();
            if (IsProcessDone(process))
            {
                CleanProcess(process);
                await TryStartNextProcess(immediately);
                return;
            }

            await StartRunningProcess(process, immediately);
            await CleanAfterRunning(process);
            await TryStartNextProcess(_queue.HasNext);
        }

        private async Task StartRunningProcess(Process process, bool immediately)
        {
            _currentProcess.Value = process;
            _cancellationTokenSource = new CancellationTokenSource();
            await RunProcess(process, _cancellationTokenSource.Token, immediately);
        }

        private async Task CleanAfterRunning(Process process)
        {
            bool cleanImmediately = _queue.HasNext || _stopped;
            await CleanOnProcessEnded(process, cleanImmediately);
            CleanCancellationTokenSource();
            CleanCurrentProcess();
        }

        private void TryStopCurrentProcess()
        {
            _stopped = true;
            _cancellationTokenSource?.Cancel();
        }

        private async void OnProcessEnqueued()
        {
            await TryStartNextProcess(immediately: false);
        }

        private void CleanCurrentProcess()
        {
            Process process = _currentProcess.Value;
            if (process == null)
            {
                return;
            }

            CleanProcess(process);
            _currentProcess.Value = null;
        }

        private void CleanProcess(Process process)
        {
            process.Dispose();
        }
        
        private void CleanCancellationTokenSource()
        {
            _cancellationTokenSource?.Dispose();
            _cancellationTokenSource = null;
        }

        private bool CanStartNextProcess()
        {
            return _queue.HasNext && _currentProcess.Value == null && !_stopped;
        }

        private bool IsProcessDone(Process process)
        {
            return process.Complete.Value || process.Stopped.Value;
        }

        protected abstract Task RunProcess(Process process, CancellationToken token, bool immediately);
        protected abstract Task CleanOnProcessEnded(Process process, bool immediately);

    }
}