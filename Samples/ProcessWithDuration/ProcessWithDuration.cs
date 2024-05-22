using System;
using System.Collections;
using UnityEngine;

namespace SBaier.Process.Samples
{
    public class ProcessWithDuration : Process
    {
        public event Action OnFinished;
        public event Action OnStopped;
        public float Progress => GetProgress();
        public bool Stopped { get; private set; }
        public string Name { get; private set; }

        private readonly float _duration;
        private readonly CoroutineHelper _coroutineHelper;
        private float? _startTime;
        private float? _stopTime;
        private Coroutine _routine;

        public ProcessWithDuration(ProcessArguments args,
            CoroutineHelper coroutineHelper)
        {
            _duration = args.Duration;
            Name = args.Name;
            _coroutineHelper = coroutineHelper;
        }
        
        public void Start()
        {
            Debug.Log($"Starting process '{Name}'");
            _startTime = Time.realtimeSinceStartup;
            _routine = _coroutineHelper.StartCoroutine(CheckFinished());
        }

        public void Stop()
        {
            Debug.Log($"Stopping process '{Name}'");
            _stopTime = Time.realtimeSinceStartup;
            Stopped = true;
            OnStopped?.Invoke();
            CleanRoutine();
        }

        private IEnumerator CheckFinished()
        {
            yield return new WaitUntil(() => GetDeltaTime() >= _duration);
            Debug.Log($"Finished process '{Name}' after {GetDeltaTime()} seconds");
            OnFinished?.Invoke();
            _routine = null;
        }

        private float GetDeltaTime()
        {
            float maxTime = _stopTime ?? Time.realtimeSinceStartup;
            float minTime = _startTime ?? Time.realtimeSinceStartup;
            return maxTime - minTime;
        }

        private float GetProgress()
        {
            return _duration != 0 ? Mathf.Clamp01(GetDeltaTime() / _duration) : 1;
        }

        private void CleanRoutine()
        {
            if (_routine == null)
            {
                return;
            }
            
            _coroutineHelper.StopCoroutine(_routine);
            _routine = null;
        }
    }
}
