namespace SBaier.Process.UI
{
    public static class ProcessNameExtensions
    {
        public static Process WithName(this Process process, string name)
        {
            process.AddProperty<ProcessName>(new BasicProcessName(name));
            return process;
        }

        public static ProcessGroup WithGroupName(this ProcessGroup group)
        {
            group.AddProperty<ProcessName>(new GroupProcessName(group));
            return group;
        }

        public static ProcessGroup WithGroupName(this ProcessGroup group, string text)
        {
            group.AddProperty<ProcessName>(new GroupProcessName(group, text));
            return group;
        }
    }
}