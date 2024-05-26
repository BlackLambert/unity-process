namespace SBaier.Process
{
    public interface ProcessGroup : Process
    {
        ReadonlyObservable<Process> CurrentProcess { get; }
        ReadonlyObservable<int> HandledProcessesAmount { get; }
        ReadonlyObservable<int> TotalProcessesAmount { get; }
    }
}