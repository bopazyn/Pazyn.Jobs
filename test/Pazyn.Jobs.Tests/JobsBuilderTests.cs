using System;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Xunit;

namespace Pazyn.Jobs.Tests
{
    public class JobsBuilderTests
    {
        public const String ValidCronExpression = "* * * * *";

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("a")]
        public void AddJob_ThrowArgumentException_CronExpressionIsInvalid(String cronExpression)
        {
            var serviceCollection = new ServiceCollection();
            var jobsBuilder = new JobsBuilder(serviceCollection);

            Assert.ThrowsAny<Exception>(() => jobsBuilder.AddJob<EmptyJob1>(cronExpression));
            Assert.ThrowsAny<Exception>(() => jobsBuilder.AddJob(cronExpression, () => new EmptyJob1()));
            Assert.ThrowsAny<Exception>(() => jobsBuilder.AddJob(cronExpression, sp => new EmptyJob1()));
        }

        [Theory]
        [InlineData(ValidCronExpression)]
        public void AddJob_CronExpressionIsValid(String cronExpression)
        {
            var serviceCollection = new ServiceCollection();
            var jobsBuilder = new JobsBuilder(serviceCollection);

            jobsBuilder.AddJob<EmptyJob1>(cronExpression);
            jobsBuilder.AddJob(cronExpression, () => new EmptyJob1());
            jobsBuilder.AddJob(cronExpression, sp => new EmptyJob1());

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var options = serviceProvider.GetRequiredService<IOptions<JobsContext>>();

            Assert.Equal(3, options.Value.JobItems.Count);
        }

        [Fact]
        public void AddJob_SameJobMultipleTimes()
        {
            var serviceCollection = new ServiceCollection();
            var jobsBuilder = new JobsBuilder(serviceCollection);

            jobsBuilder
                .AddJob<EmptyJob1>("30 0 * * *")
                .AddJob<EmptyJob1>("30 1 * * *")
                .AddJob<EmptyJob1>("30 2 * * *");

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var options = serviceProvider.GetRequiredService<IOptions<JobsContext>>();

            var localDateTimeOffset = new DateTimeOffset(DateTime.Today);
            var jobStorageItems = options.Value.GetJobsToRun(localDateTimeOffset, localDateTimeOffset.AddHours(2));

            Assert.NotEmpty(options.Value.JobItems);
            Assert.Equal(2, jobStorageItems.Length);
        }

        [Fact]
        public void AddJob_MultipleJobSameTime()
        {
            var serviceCollection = new ServiceCollection();
            var jobsBuilder = new JobsBuilder(serviceCollection);

            jobsBuilder
                .AddJob<EmptyJob1>("30 0 * * *")
                .AddJob<EmptyJob1>("30 5 * * *")
                .AddJob<EmptyJob2>("30 0 * * *")
                .AddJob<EmptyJob2>("30 5 * * *");

            var serviceProvider = serviceCollection.BuildServiceProvider();
            var options = serviceProvider.GetRequiredService<IOptions<JobsContext>>();

            var localDateTimeOffset = new DateTimeOffset(DateTime.Today);
            var jobStorageItems = options.Value.GetJobsToRun(localDateTimeOffset, localDateTimeOffset.AddHours(2));

            Assert.NotEmpty(options.Value.JobItems);
            Assert.Equal(2, jobStorageItems.Length);
        }

        [Fact]
        public void Decorate_DecoratesLastItem()
        {
            var services = new ServiceCollection();
            services.AddJobs()
                .AddJob<EmptyJob1>("30 0 * * *", ji =>
                {
                    ji.Decorate<EmptyJobDecorator1>();
                    ji.Decorate<EmptyJobDecorator2>();
                })
                .AddJob<EmptyJob2>("30 0 * * *");

            var serviceProvider = services.BuildServiceProvider();
            var options = serviceProvider.GetRequiredService<IOptions<JobsContext>>();
            var jobs = options.Value.JobItems.Select(x => x.JobFactory(serviceProvider)).ToArray();

            Assert.Collection(jobs,
                job => Assert.IsType<EmptyJobDecorator2>(job),
                job => Assert.IsType<EmptyJob2>(job));
        }

        [Fact]
        public void WithIdentifier_ThrowsException_NoJobsWasRegisteredWithIdentifier()
        {
            var services = new ServiceCollection();
            services.AddJobs()
                .AddJob<EmptyJob1>("30 0 * * *", ji => ji.Identifier = "A");

            var serviceProvider = services.BuildServiceProvider();
            var jobQueue = serviceProvider.GetRequiredService<IJobQueue>();

            Assert.Throws<InvalidOperationException>(() => jobQueue.QueueJobFromStore("B"));
        }

        [Fact]
        public void WithIdentifier_ThrowsException_QueueJobWithDuplicateIdentifier()
        {
            var services = new ServiceCollection();
            services.AddJobs()
                .AddJob<EmptyJob1>("30 0 * * *", ji => ji.Identifier = "A")
                .AddJob<EmptyJob1>("30 0 * * *", ji => ji.Identifier = "A");

            var serviceProvider = services.BuildServiceProvider();
            var jobQueue = serviceProvider.GetRequiredService<IJobQueue>();

            Assert.Throws<InvalidOperationException>(() => jobQueue.QueueJobFromStore("A"));
        }

        [Fact]
        public void WithIdentifier_Ok()
        {
            var services = new ServiceCollection();
            services.AddJobs()
                .AddJob<EmptyJob1>("30 0 * * *", ji => ji.Identifier = "A")
                .AddJob<EmptyJob1>("30 0 * * *", ji => ji.Identifier = "B");

            var serviceProvider = services.BuildServiceProvider();
            var jobQueue = serviceProvider.GetRequiredService<IJobQueue>();

            jobQueue.QueueJobFromStore("A");
            jobQueue.QueueJobFromStore("B");
        }
    }
}