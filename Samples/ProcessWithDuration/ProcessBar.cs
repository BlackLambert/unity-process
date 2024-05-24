using System;
using SBaier.DI;
using UnityEngine;
using UnityEngine.UI;

namespace SBaier.Process.Samples
{
    public class ProcessBar : MonoBehaviour, Injectable, Initializable, Cleanable
    {
        [SerializeField] 
        private Slider _slider;
        
        private Observable<Process> _currentProcess;
        
        public void Inject(Resolver resolver)
        {
            _currentProcess = resolver.Resolve<Observable<Process>>();
        }

        public void Initialize()
        {
            _currentProcess.OnValueChanged += UpdateProgress;
            UpdateProgress();
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
            UpdateProgress();
        }

        private void UpdateProgress()
        {
            _slider.value = _currentProcess.Value?.Progress ?? 0;
        }
    }
}
