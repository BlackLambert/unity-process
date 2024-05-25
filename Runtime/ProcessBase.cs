using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace SBaier.Process
{
    public abstract class ProcessBase : Process
    {
        protected const int _delayInMilliseconds = 33;

        public ReadonlyObservable<float> Progress => _progress;
        public ReadonlyObservable<bool> Stopped => _stopped;
        public ReadonlyObservable<bool> Complete => _finished;

        private readonly Observable<float> _progress = 0;
        private readonly Observable<bool> _finished = false;
        private readonly Observable<bool> _stopped = false;
        private readonly Dictionary<Type, ProcessProperty> _properties = new();

        private bool _started = false;

        public async Task Run(CancellationToken token)
        {
            ValidateRun();
            _started = true;
            Task internalTask = RunInternal(token);

            while (!internalTask.IsCanceled && 
                   !internalTask.IsCompleted && 
                   !internalTask.IsFaulted && 
                   !token.IsCancellationRequested)
            {
                await Task.Delay(_delayInMilliseconds, token);
                float progress = GetProgress();
                if (Math.Abs(_progress.Value - progress) > float.Epsilon)
                {
                    _progress.Value = GetProgress();
                }
            }

            _finished.Value = internalTask.IsCompletedSuccessfully;
            bool canceled = token is { IsCancellationRequested: true };
            _stopped.Value = internalTask.IsCanceled || canceled;
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
            if (_started)
            {
                throw new InvalidOperationException("Execute called on a process that is already running");
            }
        }

        protected abstract Task RunInternal(CancellationToken token);
        protected abstract float GetProgress();
    }
}