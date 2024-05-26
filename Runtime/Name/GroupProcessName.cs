using UnityEngine;

namespace SBaier.Process.UI
{
    public class GroupProcessName : ProcessName
    {
        private const string _defaultText = "Running process ({0}/{1}): {2}";
        
        public ReadonlyObservable<string> Name => _name;

        private readonly Observable<string> _name = new();
        private readonly ProcessGroup _group;
        private readonly string _text;

        public GroupProcessName(ProcessGroup group, string text = _defaultText)
        {
            _group = group;
            _text = text;
            _group.CurrentProcess.OnValueChanged += OnCurrentProcessChanged;
            _group.HandledProcessesAmount.OnValueChanged += OnHandledProcessesAmountChanged;
            _group.TotalProcessesAmount.OnValueChanged += OnTotalProcessesAmountChanged;
        }
        
        public void Dispose()
        {
            _group.CurrentProcess.OnValueChanged -= OnCurrentProcessChanged;
            _group.HandledProcessesAmount.OnValueChanged -= OnHandledProcessesAmountChanged;
            _group.TotalProcessesAmount.OnValueChanged -= OnTotalProcessesAmountChanged;
        }

        private void OnTotalProcessesAmountChanged(int formervalue, int newvalue)
        {
            UpdateName();
        }

        private void OnHandledProcessesAmountChanged(int formervalue, int newvalue)
        {
            UpdateName();
        }

        private void OnCurrentProcessChanged(Process formervalue, Process newvalue)
        {
            UpdateName();
        }

        private void UpdateName()
        {
            int totalAmount = _group.TotalProcessesAmount.Value;
            int processIndex = Mathf.Clamp(_group.HandledProcessesAmount.Value + 1, 0, totalAmount);
            string name = string.Empty;
            if(_group.CurrentProcess.Value != null && 
               _group.CurrentProcess.Value.TryGetProperty(out ProcessName processName))
            {
                name = processName.Name.Value;
            }
            _name.Value = string.Format(_text, processIndex, totalAmount, name);
        }
    }
}