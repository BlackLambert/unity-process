namespace SBaier.Process
{
    public static class StartTimeExtensions
    {
        public static Process WithStartTime(this Process process, Time.Time time)
        {
            process.AddProperty(new ProcessStartTime(process, time));
            return process;
        }
        
        public static Process WithUnscaledStartTime(this Process process, Time.Time time)
        {
            process.AddProperty(new ProcessStartTime(process, time, true));
            return process;
        }
    }
}