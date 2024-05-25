using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace SBaier.Process
{
    public class FixedDurationProcess : ProcessBase
    {
        private readonly float _duration;
        private float? _startTime;
        private float? _stopTime;
        
        public FixedDurationProcess(float duration)
        {
            _duration = duration;
        }
        
        protected override async Task RunInternal(CancellationToken token)
        {
            _startTime = Time.realtimeSinceStartup;
            await Task.Delay((int)(_duration * 1000), token);
        }

        protected override float GetProgress()
        {
            return Mathf.Clamp01(GetDeltaTime() / _duration);
        }

        private float GetDeltaTime()
        {
            float maxTime = _stopTime ?? Time.realtimeSinceStartup;
            float minTime = _startTime ?? Time.realtimeSinceStartup;
            return maxTime - minTime;
        }
    }
}