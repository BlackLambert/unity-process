using System.Collections.Generic;
using SBaier.DI;
using UnityEngine;

namespace SBaier.Process.Samples
{
    public class EntriesCreator : MonoBehaviour, Injectable, Initializable, Cleanable
    {
        [SerializeField] 
        private ProcessEntry _entryPrefab;

        [SerializeField] 
        private Transform _hook;
        
        private Observable<ProcessArguments> _latestArgs;
        private Observable<Process> _process;
        private List<ProcessArguments> _argumentsList;
        private List<ProcessEntry> _entries = new List<ProcessEntry>();
        
        public void Inject(Resolver resolver)
        {
            _latestArgs = resolver.Resolve<Observable<ProcessArguments>>();
            _argumentsList = resolver.Resolve<List<ProcessArguments>>();
            _process = resolver.Resolve<Observable<Process>>();
        }

        public void Initialize()
        {
            _latestArgs.OnValueChanged += OnLatestArgsChanged;
            _process.OnValueChanged += OnCurrentProcessChanged;
            CreateInitialEntries();
        }

        public void Clean()
        {
            _latestArgs.OnValueChanged -= OnLatestArgsChanged;
            _process.OnValueChanged -= OnCurrentProcessChanged;
            RemoveEntries();
        }

        private void OnLatestArgsChanged(ProcessArguments formervalue, ProcessArguments newvalue)
        {
            CreateEntry(newvalue);
        }

        private void CreateInitialEntries()
        {
            foreach (ProcessArguments arguments in _argumentsList)
            {
                CreateEntry(arguments);
            }
        }

        private void CreateEntry(ProcessArguments arguments)
        {
            ProcessEntry entry = Instantiate(_entryPrefab, _hook);
            entry.Init(arguments);
            entry.OnDelete += RemoveEntry;
            _entries.Add(entry);
        }

        private void RemoveEntries()
        {
            foreach (ProcessEntry entry in _entries)
            {
                entry.OnDelete -= RemoveEntry;
                Destroy(entry.gameObject);
            }
            _entries.Clear();
        }

        private void RemoveEntry(ProcessEntry entry)
        {
            entry.OnDelete -= RemoveEntry;
            _argumentsList.Remove(entry.Args);
            _entries.Remove(entry);
            Destroy(entry.gameObject);
        }

        private void OnCurrentProcessChanged(Process formervalue, Process newvalue)
        {
            if (newvalue != null)
            {
                RemoveEntries();
            }
        }
    }
}
