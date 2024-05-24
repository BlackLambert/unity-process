using SBaier.DI;
using TMPro;
using UnityEngine;

namespace SBaier.Process.Samples
{
    public class CurrentProcessName : MonoBehaviour, Injectable, Initializable, Cleanable
    {
        [SerializeField] private TextMeshProUGUI _text;

        private Observable<Process> _currentProcess;

        public void Inject(Resolver resolver)
        {
            _currentProcess = resolver.Resolve<Observable<Process>>();
        }

        public void Initialize()
        {
            _currentProcess.OnValueChanged += UpdateName;
            UpdateName(null, _currentProcess.Value);
        }

        public void Clean()
        {
            _currentProcess.OnValueChanged -= UpdateName;
        }

        private void UpdateName(Process formervalue, Process newvalue)
        {
            _text.text = newvalue != null
                ? $"Current process: {(newvalue as ProcessWithDuration)?.Name}"
                : string.Empty;
        }
    }
}