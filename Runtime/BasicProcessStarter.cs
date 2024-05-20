using System.Threading.Tasks;

namespace SBaier.Process
{
    public class BasicProcessStarter: ProcessStarterBase
    {
        protected override Task StartProcess(Process process, bool immediately)
        {
            process.Start();
            return Task.CompletedTask;
        }

        protected override Task CleanOnProcessEnded(Process process, bool immediately)
        {
            return Task.CompletedTask;
        }
    }
}