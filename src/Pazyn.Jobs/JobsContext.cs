using System;
using System.Collections.Generic;
using System.Linq;

namespace Pazyn.Jobs
{
    internal class JobsContext
    {
        internal JobsOptions JobsOptions { get; set; }
        internal ICollection<JobItem> JobItems { get; set; } = new HashSet<JobItem>();

        public JobItem[] GetJobsToRun(DateTimeOffset from, DateTimeOffset to) =>
            JobItems.Where(x => x.CronExpression.GetOccurrences(from, to, TimeZoneInfo.Local).Any())
                .ToArray();
    }
}