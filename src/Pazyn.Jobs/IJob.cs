using System.Threading;
using System.Threading.Tasks;

namespace Pazyn.Jobs
{
    public interface IJob
    {
        Task Run(CancellationToken cancellationToken);
    }
}