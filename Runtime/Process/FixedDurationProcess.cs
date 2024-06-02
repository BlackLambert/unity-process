using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace SBaier.Process
{
    public class FixedDurationProcess : ProcessBase
    {
        private readonly Time.Time _time;
        private readonly float _duration;
        private readonly bool _unscaled;
        private float? _startTime;
        private float? _stopTime;
        
        public FixedDurationProcess(float duration, Time.Time time, bool unscaled = false)
        {
            _duration = duration;
            _time = time;
            _unscaled = unscaled;
        }
        
        protected override async Task RunInternal(CancellationToken token)
        {
            _startTime = GetCurrentTime();
            await Task.Delay((int)(_duration * 1000), token);
        }

        protected override float GetProgress()
        {
            return Mathf.Clamp01(GetDeltaTime() / _duration);
        }

        private float GetDeltaTime()
        {
            float maxTime = _stopTime ?? GetCurrentTime();
            float minTime = _startTime ?? GetCurrentTime();
            return maxTime - minTime;
        }

        private float GetCurrentTime()
        {
            return _unscaled ? _time.CurrentUnscaledTime : _time.CurrentTime;
        }
    }
}