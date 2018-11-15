using System;

namespace IntegrationBl.Configurations
{
    public class UpdateTasksConfig
    {
        public TimeSpan UpdateInfoAboutTvShowAsyncTimeSpan { get; set; }

        public TimeSpan MinimalIntervalBetweenDelayIncreasing { get; set; }

        public int IncreaseDelayStepMilliseconds { get; set; }

    }
}
