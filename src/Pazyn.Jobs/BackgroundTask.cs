using System;
using System.Threading;
using System.Threading.Tasks;

namespace Pazyn.Jobs
{
    public class BackgroundTask
    {
        public Func<IServiceProvider, CancellationToken, Task> Delegate { get; }
        public CancellationToken CancellationToken { get; }

        public BackgroundTask(Func<IServiceProvider, CancellationToken, Task> @delegate, CancellationToken cancellationToken)
        {
            Delegate = @delegate;
            CancellationToken = cancellationToken;
        }
    }
}