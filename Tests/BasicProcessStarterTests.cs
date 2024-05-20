using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using SBaier.DI;
using UnityEngine;
using UnityEngine.TestTools;

namespace SBaier.Process.Tests
{
    public class BasicProcessStarterTests : MonoBehaviour
    {
        private static ProcessArgs[][] _initialProcessValues =
        {
            new ProcessArgs[]
            {
                new (){Progress = 1.0f, Stopped = false},
                new (){Progress = 0.34f, Stopped = true},
                new (){Progress = 0f, Stopped = false},
                new (){Progress = 0.75f, Stopped = false},
            },
            new ProcessArgs[]
            {
                new (){Progress = 1.0f, Stopped = true},
                new (){Progress = 0f, Stopped = false},
            },
            new ProcessArgs[]
            {
            },
        };
        
        private Mock<ProcessQueue> _queueMock;
        private Mock<Resolver> _resolver;
        private BasicProcessStarter _processStarter;
        private int _startedProcessesCount = 0;
        private List<Process> _processes = new ();
        private List<Process> _handledProcesses = new();
        
        [SetUp]
        public void Setup()
        {
            _processStarter = new GameObject().AddComponent<BasicProcessStarter>();
        }

        [TearDown]
        public void TearDown()
        {
            _processStarter.Clean();
            Destroy(_processStarter.gameObject);
            _processStarter = null;
            _queueMock = null;
            _startedProcessesCount = 0;
            _processes.Clear();
            _handledProcesses.Clear();
        }
        
        [UnityTest]
        public IEnumerator ProcessStarter_StartsInitialProcesses(
            [ValueSource(nameof(_initialProcessValues))]ProcessArgs[] processArgs)
        {
            Initialize(processArgs);
            yield return 0;
            Assert.AreEqual(0, _processes.Count);
            Assert.AreEqual(processArgs.Length, _handledProcesses.Count);
            Assert.True(_handledProcesses.All(process => process.Finished || process.Stopped));
        }

        [UnityTest]
        public IEnumerator ProcessStarter_SkipsFinishedAndStoppedProcesses(
            [ValueSource(nameof(_initialProcessValues))]ProcessArgs[] processArgs)
        {
            Initialize(processArgs);
            yield return 0;
            int amountToStart = processArgs.Count(arg => arg.Progress != 1.0f && !arg.Stopped);
            Assert.AreEqual(amountToStart, _startedProcessesCount);
        }
        
        [UnityTest]
        public IEnumerator ProcessStarter_StartsNewEnqueuedProcesses(
            [ValueSource(nameof(_initialProcessValues))]ProcessArgs[] processArgs)
        {
            Initialize(Array.Empty<ProcessArgs>());
            yield return 0;
            foreach (ProcessArgs newProcessArgs in processArgs)
            {
                Process newProcess = CreateProcess(newProcessArgs);
                _processes.Add(newProcess);
            }
            _queueMock.Raise(queue => queue.OnEnqueue += null);
            Assert.AreEqual(processArgs.Length, _handledProcesses.Count);
            Assert.True(_handledProcesses.All(process => process.Finished || process.Stopped));
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
                _processes.Add(CreateProcess(args));
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

        private Process CreateProcess(ProcessArgs args)
        {
            Mock<Process> processMock = new Mock<Process>();

            void ProcessStarted()
            {
                _startedProcessesCount++;
                processMock.Setup(process => process.Progress).Returns(1.0f);
                processMock.Setup(process => process.Finished).Returns(true);
                processMock.Raise(process => process.OnFinished += null);
            }
            
            processMock.Setup(process => process.Progress).Returns(args.Progress);
            processMock.Setup(process => process.Stopped).Returns(args.Stopped);
            processMock.Setup(process => process.Start()).Callback(ProcessStarted);
            processMock.Setup(process => process.Finished).Returns(args.Progress == 1.0f);
            return processMock.Object;
        }

        delegate bool TryGetReturns(out Process process);

        public struct ProcessArgs
        {
            public float Progress;
            public bool Stopped;
        }
    }
}
