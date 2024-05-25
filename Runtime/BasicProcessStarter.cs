using System.Threading;
using System.Threading.Tasks;

namespace SBaier.Process
{
    public class BasicProcessStarter: ProcessStarterBase
    {
        protected override async Task RunProcess(Process process, CancellationToken token, bool immediately)
        {
            await process.Run(token);
        }

        protected override Task CleanOnProcessEnded(Process process, bool immediately)
        {
            return Task.CompletedTask;
        }
    }
}