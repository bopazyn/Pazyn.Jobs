using System.Threading;
using System.Threading.Tasks;

namespace Pazyn.Jobs.Tests
{
    public class EmptyJob2 : IJob
    {
        public Task Run(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}