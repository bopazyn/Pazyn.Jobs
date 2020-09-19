using System.Threading;
using System.Threading.Tasks;

namespace Pazyn.Jobs.Tests
{
    public class EmptyJobDecorator2 : IJob
    {
        private IJob InnerJob { get; }

        public EmptyJobDecorator2(IJob innerJob)
        {
            InnerJob = innerJob;
        }

        public Task Run(CancellationToken cancellationToken) => InnerJob.Run(cancellationToken);
    }
}