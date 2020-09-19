using System.Threading;
using System.Threading.Tasks;

namespace Pazyn.Jobs.Tests
{
    public class EmptyJob1 : IJob
    {
        public Task Run(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}