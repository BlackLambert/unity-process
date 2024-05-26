namespace SBaier.Process.UI
{
    public class BasicProcessName : ProcessName
    {
        public ReadonlyObservable<string> Name => _name;
        private readonly Observable<string> _name = new();

        public BasicProcessName(string name)
        {
            _name.Value = name;
        }

        public void Dispose()
        {
            
        }
    }
}
