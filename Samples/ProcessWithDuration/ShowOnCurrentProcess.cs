using System.Collections;
using System.Collections.Generic;
using SBaier.DI;
using UnityEngine;

namespace SBaier.Process.Samples
{
    public class ShowOnCurrentProcess : MonoBehaviour, Injectable, Initializable, Cleanable
    {
        [SerializeField] 
        private bool _show;

        [SerializeField] 
        private GameObject _target;

        private Observable<Process> _process;
        
        public void Inject(Resolver resolver)
        {
            _process = resolver.Resolve<Observable<Process>>();
        }

        public void Initialize()
        {
            _process.OnValueChanged += OnProcessChanged;
            UpdateShow();
        }

        public void Clean()
        {
            _process.OnValueChanged -= OnProcessChanged;
        }

        private void OnProcessChanged(Process formervalue, Process newvalue)
        {
            UpdateShow();
        }

        private void UpdateShow()
        {
            bool hasProcess = _process.Value != null;
            _target.SetActive(hasProcess && _show ||
                              !hasProcess && !_show);
        }
    }
}
