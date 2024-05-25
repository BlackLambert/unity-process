using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace SBaier.Process
{
    public class AsyncOperationProcess : ProcessBase
    {
        private readonly Func<AsyncOperation> _creatOperationFunction;
        private AsyncOperation _operation;
        
        public AsyncOperationProcess(Func<AsyncOperation> creatOperationFunction)
        {
            _creatOperationFunction = creatOperationFunction;
        }

        protected override async Task RunInternal(CancellationToken token)
        {
            _operation = _creatOperationFunction();
            while (!_operation.isDone)
            {
                await Task.Delay(_delayInMilliseconds, token);
            }
        }

        protected override float GetProgress()
        {
            return _operation?.progress ?? 0;
        }
    }
}