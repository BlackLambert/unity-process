using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Moq;
using NUnit.Framework;
using SBaier.DI;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace SBaier.Process.Tests
{
    public class BasicProcessStarterTests
    {
        private static ProcessArgs[][] _initialProcessValues =
        {
            new ProcessArgs[]
            {
                new() { Progress = 1.0f, Stopped = false },
                new() { Progress = 0.34f, Stopped = true },
                new() { Progress = 0f, Stopped = false },
                new() { Progress = 0.75f, Stopped = false },
            },
            new ProcessArgs[]
            {
                new() { Progress = 1.0f, Stopped = true },
                new() { Progress = 0f, Stopped = false },
            },
            new ProcessArgs[]
            {
            },
        };

        private Mock<ProcessQueue> _queueMock;
        private Mock<Resolver> _resolver;
        private BasicProcessStarter _processStarter;
        private int _startedProcessesCount = 0;
        private readonly List<Process> _processes = new();
        private readonly List<Process> _handledProcesses = new();

        [SetUp]
        public void Setup()
        {
            _processStarter = new GameObject().AddComponent<BasicProcessStarter>();
        }

        [TearDown]
        public void TearDown()
        {
            _processStarter.Clean();
            Object.DestroyImmediate(_processStarter.gameObject);
            _processStarter = null;
            _queueMock = null;
            _startedProcessesCount = 0;
            _processes.Clear();
            _handledProcesses.Clear();
        }

        [UnityTest]
        public IEnumerator ProcessStarter_StartsInitialProcesses(
            [ValueSource(nameof(_initialProcessValues))]
            ProcessArgs[] processArgs)
        {
            Initialize(processArgs);
            yield return 0;
            Assert.AreEqual(0, _processes.Count);
            Assert.AreEqual(processArgs.Length, _handledProcesses.Count);
            Assert.True(_handledProcesses.All(process => process.Complete.Value || process.Stopped.Value));
        }

        [UnityTest]
        public IEnumerator ProcessStarter_SkipsFinishedAndStoppedProcesses(
            [ValueSource(nameof(_initialProcessValues))]
            ProcessArgs[] processArgs)
        {
            Initialize(processArgs);
            yield return 0;
            int amountToStart = processArgs.Count(arg => arg.Progress != 1.0f && !arg.Stopped);
            Assert.AreEqual(amountToStart, _startedProcessesCount);
        }

        [UnityTest]
        public IEnumerator ProcessStarter_StartsNewEnqueuedProcesses(
            [ValueSource(nameof(_initialProcessValues))]
            ProcessArgs[] processArgs)
        {
            Initialize(Array.Empty<ProcessArgs>());
            yield return 0;
            foreach (ProcessArgs newProcessArgs in processArgs)
            {
                Process newProcess = CreateProcessMock(newProcessArgs).Object;
                _processes.Add(newProcess);
            }

            _queueMock.Raise(queue => queue.OnEnqueue += null);
            Assert.AreEqual(processArgs.Length, _handledProcesses.Count);
            Assert.True(_handledProcesses.All(process => process.Complete.Value || process.Stopped.Value));
        }

        private void Initialize(ProcessArgs[] processArgs)
        {
            SetupProcesses(processArgs);
            SetupProcessQueue(_processes);
            SetupResolver(_queueMock.Object);
            _processStarter.Inject(_resolver.Object);
            _processStarter.Initialize();
        }

        private void SetupProcesses(ProcessArgs[] processArgs)
        {
            foreach (ProcessArgs args in processArgs)
            {
                _processes.Add(CreateProcessMock(args).Object);
            }
        }

        private void SetupProcessQueue(List<Process> processes)
        {
            _queueMock = new Mock<ProcessQueue>();

            Process GetNextProcess()
            {
                Process process = processes.First();
                processes.Remove(process);
                _handledProcesses.Add(process);
                return process;
            }

            bool GetNextProcessOut(out Process process)
            {
                bool hasNext = processes.Count > 0;
                process = hasNext ? GetNextProcess() : null;
                _handledProcesses.Add(process);
                return hasNext;
            }

            bool HasNextProcess()
            {
                return processes.Count > 0;
            }

            _queueMock.Setup(queue => queue.HasNext).Returns(HasNextProcess);
            _queueMock.Setup(queue => queue.Dequeue()).Returns(GetNextProcess);
            _queueMock.Setup(queue => queue.TryDequeue(out It.Ref<Process>.IsAny))
                .Returns(new TryGetReturns((out Process process) => GetNextProcessOut(out process)));
        }

        private void SetupResolver(ProcessQueue queue)
        {
            _resolver = new Mock<Resolver>();
            _resolver.Setup(resolver => resolver.Resolve<ProcessQueue>()).Returns(queue);
            _resolver.Setup(resolver => resolver.Resolve<Observable<Process>>()).Returns(new Observable<Process>());
        }

        private Mock<Process> CreateProcessMock(ProcessArgs args)
        {
            Mock<Process> processMock = CreateBasicProcessMock(args);

            void ProcessStarted()
            {
                _startedProcessesCount++;
                ((Observable<float>)processMock.Object.Progress).Value = 1.0f;
                ((Observable<bool>)processMock.Object.Complete).Value = true;
            }

            processMock.Setup(process => process.Run(It.IsAny<CancellationToken>())).Callback(ProcessStarted);
            return processMock;
        }

        private Mock<Process> CreateBasicProcessMock(ProcessArgs args)
        {
            Mock<Process> processMock = new Mock<Process>();
            processMock.Setup(process => process.Progress).Returns(CreateObservableMock(args.Progress));
            processMock.Setup(process => process.Stopped).Returns(CreateObservableMock(args.Stopped));
            processMock.Setup(process => process.Complete).Returns(CreateObservableMock(args.Progress == 1.0f));
            return processMock;
        }

        private ReadonlyObservable<T> CreateObservableMock<T>(T value)
        {
            return new Observable<T>(){Value = value};
        }

        delegate bool TryGetReturns(out Process process);

        public struct ProcessArgs
        {
            public float Progress;
            public bool Stopped;
        }
    }
}