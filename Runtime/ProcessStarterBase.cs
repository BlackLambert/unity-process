using System.Threading.Tasks;
using SBaier.DI;
using UnityEngine;

namespace SBaier.Process
{
    public abstract class ProcessStarterBase : MonoBehaviour, Injectable, Initializable, Cleanable
    {
        private ProcessQueue _queue;
        private Observable<Process> _currentProcess;

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

        private void Update()
        {
            _currentProcess?.Value?.Update();
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
            if (process.Finished.Value || process.Stopped.Value)
            {
                await TryStartNextProcess(immediately);
                return;
            }
            
            AddListeners(process);
            _currentProcess.Value = process;
            await StartProcess(process, immediately);
        }

        private async Task TryStopCurrentProcess()
        {
            Process process = _currentProcess.Value;
            if (process == null)
            {
                return;
            }

            RemoveListeners(process);
            process.Stop();
            await CleanOnProcessEnded(process, true);
            _currentProcess.Value = null;
        }

        private void AddListeners(Process process)
        {
            process.Stopped.OnValueChanged += OnProcessEnded;
            process.Finished.OnValueChanged += OnProcessEnded;
        }

        private void RemoveListeners(Process process)
        {
            process.Stopped.OnValueChanged -= OnProcessEnded;
            process.Finished.OnValueChanged -= OnProcessEnded;
        }

        private async void OnProcessEnqueued()
        {
            await TryStartNextProcess(immediately: false);
        }

        private async void OnProcessEnded(bool formerValue, bool newValue)
        {
            if (!newValue)
            {
                return;
            }
            
            Process process = _currentProcess.Value;
            bool queueHasNext = _queue.HasNext;
            RemoveListeners(process);
            await CleanOnProcessEnded(process: process, immediately: queueHasNext);
            _currentProcess.Value = null;
            await TryStartNextProcess(immediately: queueHasNext);
        }

        protected abstract Task StartProcess(Process process, bool immediately);
        protected abstract Task CleanOnProcessEnded(Process process, bool immediately);
    }
}