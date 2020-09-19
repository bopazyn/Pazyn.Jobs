using System;
using Cronos;
using Xunit;

namespace Pazyn.Jobs.Tests
{
    public class JobsStorageTests
    {
        private JobItem Midnight = new JobItem(sp => new EmptyJob1(), CronExpression.Parse("@midnight"));
        private JobItem EverydayAt0_OClock = new JobItem(sp => new EmptyJob1(), CronExpression.Parse("0 0 * * *"));
        private JobItem EverydayAt4_OClock = new JobItem(sp => new EmptyJob1(), CronExpression.Parse("0 4 * * *"));

        [Fact]
        public void GetJobsToRun_ReturnsJobs()
        {
            var dateTimeOffset = new DateTimeOffset(DateTime.Today);
            var fromDateTimeOffset = dateTimeOffset.AddHours(-2);
            var toDateTimeOffset = dateTimeOffset.AddHours(4).AddMilliseconds(-1);

            var jobsContext = new JobsContext();
            foreach (var jobItem in new[]
            {
                Midnight,
                EverydayAt0_OClock,
                EverydayAt4_OClock
            })
            {
                jobsContext.JobItems.Add(jobItem);
            }

            var jobStorageItems = jobsContext.GetJobsToRun(fromDateTimeOffset, toDateTimeOffset);

            Assert.Equal(3, jobsContext.JobItems.Count);
            Assert.Equal(2, jobStorageItems.Length);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(10)]
        [InlineData(100)]
        public void GetJobsToRun_CronWorksInLocalTime(Int32 skip)
        {
            var dateTimeOffset = DateTimeOffset.Now;
            var fromDateTimeOffset = dateTimeOffset.AddMinutes(-skip);
            var toDateTimeOffset = dateTimeOffset.AddMinutes(skip);

            var jobStorageItem = new JobItem(sp => new EmptyJob1(), CronExpression.Parse($"{dateTimeOffset.Minute} {dateTimeOffset.Hour} * * *"));
            var jobsContext = new JobsContext();
            jobsContext.JobItems.Add(jobStorageItem);

            var jobStorageItems = jobsContext.GetJobsToRun(fromDateTimeOffset, toDateTimeOffset);

            Assert.Single(jobStorageItems);
        }
    }
}