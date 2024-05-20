using System;

namespace SBaier.Process
{
    public interface Process
    {
        event Action OnFinished;
        event Action OnStopped;
        float Progress { get; }
        bool Stopped { get; }
        bool Finished => Math.Abs(Progress - 1) < float.Epsilon;
        void Start();
        void Stop();
    }
}
