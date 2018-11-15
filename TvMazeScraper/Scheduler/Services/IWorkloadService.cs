using System;

namespace Scheduler.Services
{
    public interface IWorkloadService
    {
        TimeSpan IndividualSagaItemTaskExecutionDelay { get; }

        void IncreaseDelayTime();
    }
}