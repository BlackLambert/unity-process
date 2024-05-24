using System;
using Moq;
using NUnit.Framework;

namespace SBaier.Process.Tests
{
    public class BasicProcessQueueTests
    {
        private static object[] _processValues =
        {
            new object[]{0.3f, true},
            new object[]{1f, false},
            new object[]{0f, false},
        };
        private ProcessQueue _queue;
        
        [SetUp]
        public void Setup()
        {
            _queue = new BasicProcessQueue();
        }

        [TearDown]
        public void TearDown()
        {
            _queue = null;
        }
        
        [Test, TestCaseSource(nameof(_processValues))]
        public void ProcessQueue_DequeueReturnsEnqueuedProcess(float progress, bool stopped)
        {
            Process process = CreateNewProcess(progress, stopped);
            _queue.Enqueue(process);
            Process dequeuedProcess = _queue.Dequeue();
            Assert.AreSame(process, dequeuedProcess);
        }
        
        [Test, TestCaseSource(nameof(_processValues))]
        public void ProcessQueue_TryDequeueReturnsEnqueuedProcess(float progress, bool stopped)
        {
            Process process = CreateNewProcess(progress, stopped);
            _queue.Enqueue(process);
            _queue.TryDequeue(out Process dequeuedProcess);
            Assert.AreSame(process, dequeuedProcess);
        }
        
        [Test, TestCaseSource(nameof(_processValues))]
        public void ProcessQueue_TryDequeueReturnsTrueIfSuccessful(float progress, bool stopped)
        {
            Process process = CreateNewProcess(progress, stopped);
            _queue.Enqueue(process);
            bool success = _queue.TryDequeue(out Process _);
            Assert.True(success);
        }
        
        [Test, TestCaseSource(nameof(_processValues))]
        public void ProcessQueue_TryDequeueReturnsFalseIfUnsuccessful(float progress, bool stopped)
        {
            bool success = _queue.TryDequeue(out Process _);
            Assert.False(success);
        }
        
        [Test, TestCaseSource(nameof(_processValues))]
        public void ProcessQueue_HasNextReturnsFalseIfNoProcess(float progress, bool stopped)
        {
            Assert.False(_queue.HasNext);
        }
        
        [Test, TestCaseSource(nameof(_processValues))]
        public void ProcessQueue_HasNextReturnsTrueIfHasNextProcess(float progress, bool stopped)
        {
            Process process = CreateNewProcess(progress, stopped);
            _queue.Enqueue(process);
            Assert.True(_queue.HasNext);
        }
        
        [Test, TestCaseSource(nameof(_processValues))]
        public void ProcessQueue_EnqueueCallsOnEnqueueEvent(float progress, bool stopped)
        {
            Process process = CreateNewProcess(progress, stopped);
            bool called = false;
            Action callback = () => called = true;
            _queue.OnEnqueue += callback;
            _queue.Enqueue(process);
            _queue.OnEnqueue -= callback;
            Assert.True(called);
        }

        private Process CreateNewProcess(float progress, bool stopped)
        {
            Mock<Process> mock = new Mock<Process>();
            mock.Setup(process => process.Progress.Value).Returns(progress);
            mock.Setup(process => process.Stopped.Value).Returns(stopped);
            return mock.Object;
        }
    }
}
