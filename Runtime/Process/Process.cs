using System;
using System.Threading;
using System.Threading.Tasks;

namespace SBaier.Process
{
    public interface Process : IDisposable
    {
        ReadonlyObservable<float> Progress { get; }
        ReadonlyObservable<bool> Stopped { get; }
        ReadonlyObservable<bool> Complete { get; }
        Task Run(CancellationToken token);
        bool TryGetProperty<TProperty>(out TProperty property) where TProperty : ProcessProperty;
        void AddProperty<TProperty>(TProperty property) where TProperty : ProcessProperty;
    }
}
