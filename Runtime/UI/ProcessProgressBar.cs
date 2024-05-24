using SBaier.DI;
using UnityEngine;
using UnityEngine.UI;

namespace SBaier.Process.UI
{
    public class ProcessProgressBar : MonoBehaviour, Injectable, Initializable, Cleanable
    {
        [SerializeField] 
        private Slider _slider;
        
        private ReadonlyObservable<Process> _currentProcess;
        
        public void Inject(Resolver resolver)
        {
            _currentProcess = resolver.Resolve<Observable<Process>>();
        }

        public void Initialize()
        {
            _currentProcess.OnValueChanged += UpdateProgress;
            UpdateProgress();
            AddListeners(_currentProcess.Value);
        }

        private void Update()
        {
            UpdateProgress();
        }

        public void Clean()
        {
            _currentProcess.OnValueChanged -= UpdateProgress;
        }

        private void UpdateProgress(Process formervalue, Process newvalue)
        {
            RemoveListeners(formervalue);
            UpdateProgress();
            AddListeners(newvalue);
        }

        private void RemoveListeners(Process process)
        {
            if (process != null)
            {
                process.Progress.OnValueChanged -= UpdateProgress;
            }
        }

        private void AddListeners(Process process)
        {
            if (process != null)
            {
                process.Progress.OnValueChanged += UpdateProgress;
            }
        }

        private void UpdateProgress(float formervalue, float newvalue)
        {
            UpdateProgress();
        }

        private void UpdateProgress()
        {
            _slider.value = _currentProcess.Value?.Progress.Value ?? 0;
        }
    }
}
