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

        public async void Clean()
        {
            _queue.OnEnqueue -= OnProcessEnqueued;
            await TryStopCurrentProcess();
        }

        private async Task TryStartNextProcess(bool immediately)
        {
            if (!_queue.HasNext || _currentProcess.Value != null)
            {
                return;
            }
            
            Process process = _queue.Dequeue();
            if (process.Complete.Value || process.Stopped.Value)
            {
                await TryStartNextProcess(immediately);
                return;
            }
            
            _currentProcess.Value = process;
            _cancellationTokenSource = new CancellationTokenSource();
            await RunProcess(process, _cancellationTokenSource.Token, immediately);
            bool queueHasNext = _queue.HasNext;
            await CleanOnProcessEnded(process: process, immediately: queueHasNext);
            CleanCancellationToken();
            _currentProcess.Value = null;
            await TryStartNextProcess(queueHasNext);
        }

        private async Task TryStopCurrentProcess()
        {
            Process process = _currentProcess.Value;
            if (process == null)
            {
                return;
            }

            _cancellationTokenSource.Cancel();
            await CleanOnProcessEnded(process, true);
            _currentProcess.Value = null;
        }

        private async void OnProcessEnqueued()
        {
            await TryStartNextProcess(immediately: false);
        }
        
        private void CleanCancellationToken()
        {
            if (_cancellationTokenSource == null)
            {
                return;
            }
            
            _cancellationTokenSource.Dispose();
            _cancellationTokenSource = null;
        }

        protected abstract Task RunProcess(Process process, CancellationToken token, bool immediately);
        protected abstract Task CleanOnProcessEnded(Process process, bool immediately);

    }
}