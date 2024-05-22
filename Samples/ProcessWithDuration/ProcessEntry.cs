using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SBaier.Process.Samples
{
    public class ProcessEntry : MonoBehaviour
    {
        public event Action<ProcessEntry> OnDelete;
        public ProcessArguments Args { get; private set; }

        [SerializeField] 
        private Button _delteButton;

        [SerializeField] 
        private TextMeshProUGUI _label;

        public void Init(ProcessArguments args)
        {
            Args = args;
            _label.text = $"{args.Name} ({args.Duration}s)";
        }

        private void Start()
        {
            _delteButton.onClick.AddListener(InvokeOnDelete);
        }

        private void OnDestroy()
        {
            _delteButton.onClick.RemoveListener(InvokeOnDelete);
        }

        private void InvokeOnDelete()
        {
            OnDelete?.Invoke(this);
        }
    }
}
