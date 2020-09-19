using System;
using System.Threading.Tasks;
using Xunit;

namespace Pazyn.Jobs.Tests
{
    public class BackgroundTasksQueueTests
    {
        [Fact]
        public void QueueBackgroundTask_ThrowArgumentNullException_TaskIsNull()
        {
            var backgroundTasksQueue = new BackgroundTasksQueue();

            Assert.Throws<ArgumentNullException>(() => backgroundTasksQueue.QueueBackgroundTask(null));
        }

        [Fact]
        public void QueueBackgroundTask_ArgumentsAreOk()
        {
            var backgroundTasksQueue = new BackgroundTasksQueue();

            backgroundTasksQueue.QueueBackgroundTask((sp, ct) => Task.CompletedTask);
        }

        [Fact]
        public async Task DequeueAsync_ReturnTaskInGoodOrder()
        {
            var backgroundTasksQueue = new BackgroundTasksQueue();

            var tmp = 0;
            backgroundTasksQueue.QueueBackgroundTask((sp, ct) =>
            {
                tmp = 1;
                return Task.CompletedTask;
            });
            backgroundTasksQueue.QueueBackgroundTask((sp, ct) =>
            {
                tmp = 2;
                return Task.CompletedTask;
            });

            Assert.Equal(0, tmp);
            var backgroundTaskItem1 = await backgroundTasksQueue.DequeueAsync();
            await backgroundTaskItem1.Delegate.Invoke(null, default);
            Assert.Equal(1, tmp);

            var backgroundTaskItem2 = await backgroundTasksQueue.DequeueAsync();
            await backgroundTaskItem2.Delegate.Invoke(null, default);
            Assert.Equal(2, tmp);
        }

        [Fact]
        public async Task QueueBackgroundTask_CancelTask_Timeout()
        {
            var backgroundTasksQueue = new BackgroundTasksQueue();

            var cancelled = false;
            Boolean? completed = null;
            backgroundTasksQueue.QueueBackgroundTask(async (sp, ct) =>
            {
                try
                {
                    await Task.Delay(10000, ct);
                    completed = true;
                }
                catch (TaskCanceledException)
                {
                    cancelled = true;
                }
            }, TimeSpan.FromMilliseconds(1));

            var backgroundTaskItem = await backgroundTasksQueue.DequeueAsync();
            await backgroundTaskItem.Delegate.Invoke(null, backgroundTaskItem.CancellationToken);

            Assert.True(cancelled);
            Assert.True(!completed.HasValue);
        }
    }
}