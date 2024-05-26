using System;
using System.Collections.Generic;

namespace SBaier.Process
{
    public class BasicProcessQueue : ProcessQueue
    {
        public event Action OnEnqueue; 
        public bool HasNext => _queue.Count > 0;
        private Queue<Process> _queue = new();
        
        public void Enqueue(Process process)
        {
            _queue.Enqueue(process);
            OnEnqueue?.Invoke();
        }

        public Process Dequeue()
        {
            return _queue.Dequeue();
        }

        public bool TryDequeue(out Process process)
        {
            return _queue.TryDequeue(out process);
        }
    }
}