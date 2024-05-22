using System;
using System.Collections.Generic;
using SBaier.DI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SBaier.Process.Samples
{
    public class AddProcessButton : MonoBehaviour, Injectable
    {
        [SerializeField] 
        private TMP_InputField _nameInput;
        
        [SerializeField] 
        private TMP_InputField _durationInput;

        [SerializeField] 
        private Button _button;

        private Observable<ProcessArguments> _processArguments;
        private List<ProcessArguments> _argumentsList;

        public void Inject(Resolver resolver)
        {
            _processArguments = resolver.Resolve<Observable<ProcessArguments>>();
            _argumentsList = resolver.Resolve<List<ProcessArguments>>();
        }
        
        private void Start()
        {
            UpdateInteractable();
            _nameInput.onValueChanged.AddListener(UpdateInteractable);
            _durationInput.onValueChanged.AddListener(UpdateInteractable);
            _button.onClick.AddListener(AddProcess);
        }

        private void OnDestroy()
        {
            _nameInput.onValueChanged.RemoveListener(UpdateInteractable);
            _durationInput.onValueChanged.RemoveListener(UpdateInteractable);
            _button.onClick.RemoveListener(AddProcess);
        }

        private void UpdateInteractable(string _ = null)
        {
            _button.interactable =
                !string.IsNullOrWhiteSpace(_nameInput.text) &&
                !string.IsNullOrWhiteSpace(_durationInput.text);
        }

        private void AddProcess()
        {
            string name = _nameInput.text;
            float.TryParse(_durationInput.text, out float duration);
            ProcessArguments arguments = new ProcessArguments { Name = name, Duration = duration };
            _processArguments.Value = arguments;
            _argumentsList.Add(arguments);
            _nameInput.text = string.Empty;
            _durationInput.text = string.Empty;
        }
    }
}
