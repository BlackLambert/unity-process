using System.Collections.Generic;
using SBaier.DI;
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
                _queue.Enqueue(new ProcessWithDuration(arguments));
            }
            _processArgumentsList.Clear();
        }
    }
}
