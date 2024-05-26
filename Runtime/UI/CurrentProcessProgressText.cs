using SBaier.DI;
using TMPro;
using UnityEngine;

namespace SBaier.Process.UI
{
    public class CurrentProcessProgressText : MonoBehaviour, Injectable, Initializable, Cleanable
    {
        [SerializeField]
        private TextMeshProUGUI _textField;

        [SerializeField] 
        private string _baseText = "{0}%";

        [SerializeField] 
        private bool _usePercentage = true;
        
        private ReadonlyObservable<Process> _currentProcess;
        
        public void Inject(Resolver resolver)
        {
            _currentProcess = resolver.Resolve<ReadonlyObservable<Process>>();
        }

        private void Reset()
        {
            _textField = GetComponent<TextMeshProUGUI>();
        }

        public void Initialize()
        {
            _currentProcess.OnValueChanged += OnProcessChanged;
            AddProcessListeners(_currentProcess.Value);
            UpdateText();
        }

        public void Clean()
        {
            _currentProcess.OnValueChanged -= OnProcessChanged;
            RemoveProcessListeners(_currentProcess.Value);
        }

        private void OnProcessChanged(Process formervalue, Process newvalue)
        {
            RemoveProcessListeners(formervalue);
            UpdateText();
            AddProcessListeners(newvalue);
        }

        private void AddProcessListeners(Process process)
        {
            if (process != null)
            {
                process.Progress.OnValueChanged += OnProgressChanged;
            }
        }

        private void RemoveProcessListeners(Process process)
        {
            if (process != null)
            {
                process.Progress.OnValueChanged -= OnProgressChanged;
            }
        }

        private void OnProgressChanged(float formervalue, float newvalue)
        {
            UpdateText();
        }

        private void UpdateText()
        {
            float progress = _currentProcess.Value?.Progress.Value ?? 0;
            string percentage = _usePercentage ? ((int)(progress * 100)).ToString() : progress.ToString("F2");
            _textField.text = string.Format(_baseText, percentage);
        }
    }
}