using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace SBaier.Process
{
    public abstract class ProcessBase : Process
    {
        protected const int _delayInMilliseconds = 33;

        public event Action OnStarted;
        
        public ReadonlyObservable<float> Progress => _progress;
        public ReadonlyObservable<bool> Stopped => _stopped;
        public ReadonlyObservable<bool> Started => _started;
        public ReadonlyObservable<bool> Complete => _finished;

        private readonly Observable<float> _progress = 0;
        private readonly Observable<bool> _finished = false;
        private readonly Observable<bool> _stopped = false;
        private readonly Observable<bool> _started = false;
        private readonly Dictionary<Type, ProcessProperty> _properties = new();
        
        public virtual void Dispose()
        {
            foreach (ProcessProperty property in _properties.Values)
            {
                property.Dispose();
            }
        }

        public async Task Run(CancellationToken token)
        {
            ValidateRun();
            _started.Value = true;
            Task internalTask = RunInternal(token);
            OnStarted?.Invoke();

            while (!IsDone(internalTask, token))
            {
                await Task.Yield();
                UpdateProgress();
            }

            TryLogError(internalTask);
            UpdateFinished(internalTask);
            UpdateStopped(internalTask, token);
            internalTask.Dispose();
        }

        public bool TryGetProperty<TProperty>(out TProperty property) where TProperty : ProcessProperty
        {
            bool found = _properties.TryGetValue(typeof(TProperty), out ProcessProperty propertyBase);
            property = (TProperty)propertyBase;
            return found;
        }

        public void AddProperty<TProperty>(TProperty property) where TProperty : ProcessProperty
        {
            _properties[typeof(TProperty)] = property;
        }

        private void ValidateRun()
        {
            if (_started.Value)
            {
                throw new InvalidOperationException("Execute called on a process that is already running");
            }
        }

        private void UpdateProgress()
        {
            float progress = GetProgress();
            if (Math.Abs(_progress.Value - progress) > float.Epsilon)
            {
                _progress.Value = GetProgress();
            }
        }

        private void UpdateFinished(Task task)
        {
            _finished.Value = task.IsCompletedSuccessfully;
        }

        private void UpdateStopped(Task task, CancellationToken token)
        {
            _stopped.Value = task.IsCanceled || token is { IsCancellationRequested: true };
        }

        private bool IsDone(Task task, CancellationToken token)
        {
            return IsDone(task) || token.IsCancellationRequested;
        }

        private bool IsDone(Task task)
        {
            return task.IsCompleted || task.IsFaulted || task.IsCanceled;
        }

        private void TryLogError(Task task)
        {
            if (task.IsFaulted)
            {
                Debug.LogError($"Failed to run process: {task.Exception?.Message ?? string.Empty}");
            }
        }

        protected abstract Task RunInternal(CancellationToken token);
        protected abstract float GetProgress();

    }
}