namespace SBaier.Process
{
    public interface Process
    {
        ReadonlyObservable<float> Progress { get; }
        ReadonlyObservable<bool> Stopped { get; }
        ReadonlyObservable<bool> Finished { get; }
        void Start();
        void Update();
        void Stop();
        bool TryGetProperty<TProperty>(out TProperty property) where TProperty : ProcessProperty;
    }
}
