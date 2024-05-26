namespace SBaier.Process
{
    public interface ProcessGroup
    {
        ReadonlyObservable<Process> CurrentProcess { get; }
        ReadonlyObservable<int> HandledProcessesAmount { get; }
        ReadonlyObservable<int> TotalProcessesAmount { get; }
    }
}