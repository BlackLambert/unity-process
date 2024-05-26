using System;

namespace SBaier.Process
{
    public interface ProcessQueue
    {
        event Action OnEnqueue; 
        bool HasNext { get; }
        void Enqueue(Process process);
        Process Dequeue();
        bool TryDequeue(out Process process);
    }
}