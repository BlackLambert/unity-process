namespace SBaier.Process
{
    public class ProcessStartTime : ProcessProperty
    {
        public Observable<float?> StartTime { get; }
        public bool Unscaled { get; }
        
        private readonly Process _process;
        private readonly Time.Time _time;
        
        public ProcessStartTime(Process process, Time.Time time, bool unscaled = false)
        {
            _process = process;
            _time = time;
            Unscaled = unscaled;

            StartTime = new Observable<float?>() { Value = process.Started.Value ? GetCurrentTime() : null };
            _process.Started.OnValueChanged += OnStartedChanged;
        }

        public void Dispose()
        {
            _process.Started.OnValueChanged -= OnStartedChanged;
        }

        private void OnStartedChanged(bool formervalue, bool newvalue)
        {
            if (newvalue)
            {
                StartTime.Value = GetCurrentTime();
            }
        }

        private float GetCurrentTime()
        {
            return Unscaled ? _time.CurrentUnscaledTime : _time.CurrentTime;
        }
    }
}