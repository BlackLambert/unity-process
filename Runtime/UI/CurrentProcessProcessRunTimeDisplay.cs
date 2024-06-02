using SBaier.DI;
using TMPro;
using UnityEngine;

namespace SBaier.Process.UI
{
    public class CurrentProcessProcessRunTimeDisplay : MonoBehaviour, Injectable, Initializable, Cleanable
    {
        [SerializeField] private TextMeshProUGUI _textField;

        [SerializeField] private string _format = "F1";

        [SerializeField] private string _textBase = "{0}s";

        private ReadonlyObservable<Process> _currentProcess;
        private ProcessStartTime _startTime;
        private Time.Time _time;

        public void Inject(Resolver resolver)
        {
            _currentProcess = resolver.Resolve<ReadonlyObservable<Process>>();
            _time = resolver.Resolve<Time.Time>();
        }

        private void Reset()
        {
            _textField = GetComponent<TextMeshProUGUI>();
        }

        public void Initialize()
        {
            ResetText();
            UpdateStartTime();
            _currentProcess.OnValueChanged += OnProcessChanged;
        }

        public void Clean()
        {
            _currentProcess.OnValueChanged -= OnProcessChanged;
        }

        private void Update()
        {
            UpdateText();
        }

        private void OnProcessChanged(Process formervalue, Process newvalue)
        {
            ResetText();
            UpdateStartTime();
        }

        private void UpdateStartTime()
        {
            _currentProcess.Value?.TryGetProperty(out _startTime);
        }

        private void UpdateText()
        {
            if (_startTime?.StartTime.Value == null)
            {
                return;
            }

            float startTime = _startTime.StartTime.Value.Value;
            string valueText = (GetCurrentTime() - startTime).ToString(_format);
            _textField.text = string.Format(_textBase, valueText);
        }

        private void ResetText()
        {
            _textField.text = string.Format(_textBase, 0f.ToString(_format));
        }

        private float GetCurrentTime()
        {
            return _startTime.Unscaled ? _time.CurrentUnscaledTime : _time.CurrentTime;
        }
    }
}