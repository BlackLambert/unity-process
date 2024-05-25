using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SBaier.DI;
using SBaier.Process.UI;
using UnityEngine;
using UnityEngine.UI;

namespace SBaier.Process.Samples
{
    public class EnqueueButton : MonoBehaviour, Injectable
    {
        [SerializeField] 
        private Button _button;
        
        private List<ProcessArguments> _processArgumentsList;
        private ProcessQueue _queue;
        private CoroutineHelper _coroutineHelper;
        
        public void Inject(Resolver resolver)
        {
            _coroutineHelper = resolver.Resolve<CoroutineHelper>();
            _processArgumentsList = resolver.Resolve<List<ProcessArguments>>();
            _queue = resolver.Resolve<ProcessQueue>();
        }

        private void Start()
        {
            _button.onClick.AddListener(Enqueue);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(Enqueue);
        }

        private void Update()
        {
            _button.interactable = _processArgumentsList is { Count: > 0 };
        }

        private void Enqueue()
        {
            foreach (ProcessArguments arguments in _processArgumentsList)
            {
                _queue.Enqueue(CreateFixedDurationProcess(arguments));
                _queue.Enqueue(CreateTaskProcess(arguments));
            }
            _queue.Enqueue(CreateAsyncGroupProcess(_processArgumentsList));
            _queue.Enqueue(CreateSyncGroupProcess(_processArgumentsList));
            _processArgumentsList.Clear();
        }

        private Process CreateFixedDurationProcess(ProcessArguments arguments)
        {
            FixedDurationProcess process = new FixedDurationProcess(arguments.Duration);
            process.AddProperty(new ProcessName(arguments.Name + " (Duration)"));
            return process;
        }

        private Process CreateTaskProcess(ProcessArguments arguments)
        {
            TaskProcess process = new TaskProcess(() => Task.Delay((int)(arguments.Duration * 1000)));
            process.AddProperty(new ProcessName(arguments.Name + " (Task)"));
            return process;
        }

        private Process CreateAsyncGroupProcess(List<ProcessArguments> processArgumentsList)
        {
            IEnumerable<FixedDurationProcess> processes = 
                processArgumentsList.Select(args => new FixedDurationProcess(args.Duration));
            AsynchronousProcessGroup process = new AsynchronousProcessGroup(processes.ToList());
            process.AddProperty(new ProcessName("Async group"));
            return process;
        }

        private Process CreateSyncGroupProcess(List<ProcessArguments> processArgumentsList)
        {
            IEnumerable<FixedDurationProcess> processes = 
                processArgumentsList.Select(args => new FixedDurationProcess(args.Duration));
            SynchronousProcessGroup process = new SynchronousProcessGroup(processes.ToList());
            process.AddProperty(new ProcessName("Sync group"));
            return process;
        }
    }
}
