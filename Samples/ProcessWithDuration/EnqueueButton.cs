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
        private SBaier.Time.Time _time;
        
        public void Inject(Resolver resolver)
        {
            _processArgumentsList = resolver.Resolve<List<ProcessArguments>>();
            _queue = resolver.Resolve<ProcessQueue>();
            _time = resolver.Resolve<SBaier.Time.Time>();
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
            return new FixedDurationProcess(arguments.Duration, _time, true)
                .WithName(arguments.Name + " (Duration)")
                .WithStartTime(_time);
        }

        private Process CreateTaskProcess(ProcessArguments arguments)
        {
            return new TaskProcess(() => Task.Delay((int)(arguments.Duration * 1000)))
                    .WithName(arguments.Name + " (Task)")
                    .WithUnscaledStartTime(_time);
        }

        private Process CreateAsyncGroupProcess(List<ProcessArguments> processArgumentsList)
        {
            return new AsynchronousProcessGroup(processArgumentsList.Select(CreateFixedDurationProcess).ToList())
                    .WithGroupName("Running parallel process ({0}/{1}): {2}")
                    .WithStartTime(_time);
        }

        private Process CreateSyncGroupProcess(List<ProcessArguments> processArgumentsList)
        {
            return new SynchronousProcessGroup(processArgumentsList.Select(CreateFixedDurationProcess).ToList())
                .WithGroupName("Running sequential process ({0}/{1}): {2}")
                .WithStartTime(_time);
        }
    }
}
