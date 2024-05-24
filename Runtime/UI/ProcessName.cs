namespace SBaier.Process.UI
{
    public class ProcessName : ProcessProperty
    {
        public Observable<string> Name { get; }

        public ProcessName(string name)
        {
            Name = new Observable<string>() { Value = name };
        }
    }
}