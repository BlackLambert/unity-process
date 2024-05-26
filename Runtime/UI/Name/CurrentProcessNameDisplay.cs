using SBaier.DI;
using TMPro;
using UnityEngine;

namespace SBaier.Process.UI
{
    public class CurrentProcessNameDisplay : MonoBehaviour, Injectable, Initializable, Cleanable
    {
        [SerializeField] private TextMeshProUGUI _text;

        private ReadonlyObservable<Process> _currentProcess;
        private ProcessName _processName = null;

        public void Inject(Resolver resolver)
        {
            _currentProcess = resolver.Resolve<ReadonlyObservable<Process>>();
        }

        public void Initialize()
        {
            _currentProcess.OnValueChanged += OnProcessChanged;
            InitProcessNameOf(_currentProcess.Value);
            UpdateName();
        }

        public void Clean()
        {
            _currentProcess.OnValueChanged -= OnProcessChanged;
        }

        private void InitProcessNameOf(Process process)
        {
            process?.TryGetProperty(out _processName);
            if (_processName != null)
            {
                _processName.Name.OnValueChanged += OnNameChanged;
            }
        }

        private void OnProcessChanged(Process formervalue, Process newvalue)
        {
            RemoveListeners();
            InitProcessNameOf(newvalue);
            UpdateName();
        }

        private void RemoveListeners()
        {
            if (_processName != null)
            {
                _processName.Name.OnValueChanged -= OnNameChanged;
            }
        }

        private void UpdateName()
        {
            _text.text = _processName?.Name.Value ?? string.Empty;
        }

        private void OnNameChanged(string formervalue, string newvalue)
        {
            UpdateName();
        }
    }
}