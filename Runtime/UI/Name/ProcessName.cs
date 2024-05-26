namespace SBaier.Process.UI
{
    public interface ProcessName : ProcessProperty
    {
        public ReadonlyObservable<string> Name { get; }
    }
}