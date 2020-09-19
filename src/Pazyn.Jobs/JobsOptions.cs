using System;

namespace Pazyn.Jobs
{
    public class JobsOptions
    {
        public TimeSpan Period { get; set; } = TimeSpan.FromSeconds(5);
    }
}