using System;

namespace Scheduler.Configurations
{
    public class JobsConfig
    {
        public int StartUpdateProcessTaskMinutesInterval { get; set; }

        public TimeSpan UpdateInfoAboutTvShowAsyncTimeSpan { get; set; }

        public TimeSpan MinimalIntervalBetweenDelayIncreasing { get; set; }

        public int IncreaseDelayStepMilliseconds { get; set; }
    }
}
