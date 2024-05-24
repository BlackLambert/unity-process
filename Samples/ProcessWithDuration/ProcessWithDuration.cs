using SBaier.Process.UI;
using UnityEngine;

namespace SBaier.Process.Samples
{
    public class ProcessWithDuration : Process
    {
        public ReadonlyObservable<float> Progress => _progress;
        public ReadonlyObservable<bool> Stopped => _stopped;
        public ReadonlyObservable<bool> Finished => _finished;
        
        private Observable<float> _progress = new Observable<float>();
        private Observable<bool> _stopped = new Observable<bool>();
        private Observable<bool> _finished = new Observable<bool>();
        private readonly float _duration;
        private float? _startTime;
        private float? _stopTime;
        private ProcessName _name;

        public ProcessWithDuration(ProcessArguments args)
        {
            _duration = args.Duration;
            _name = new ProcessName(args.Name);
        }
        
        public void Start()
        {
            Debug.Log($"Starting process '{_name}'");
            _startTime = Time.realtimeSinceStartup;
            Update();
        }

        public void Update()
        {
            float deltaTime = GetDeltaTime();
            _progress.Value = _duration != 0 ? Mathf.Clamp01(deltaTime / _duration) : 1;
            if (deltaTime >= _duration)
            {
                _finished.Value = true;
            }
        }

        public void Stop()
        {
            Debug.Log($"Stopping process '{_name.Name}'");
            _stopTime = Time.realtimeSinceStartup;
            _stopped.Value = true;
        }

        public bool TryGetProperty<TProperty>(out TProperty property) where TProperty : ProcessProperty
        {
            property = default;
            if (_name is TProperty nameProperty)
            {
                property = nameProperty;
                return true;
            }
            return false;
        }

        private float GetDeltaTime()
        {
            float maxTime = _stopTime ?? Time.realtimeSinceStartup;
            float minTime = _startTime ?? Time.realtimeSinceStartup;
            return maxTime - minTime;
        }
    }
}
