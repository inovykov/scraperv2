using System;
using System.Collections.Generic;
using Shared.Models.Integration;

namespace IntegrationBl.Services
{
    public class IntegrationTaskFactory : IIntegrationTaskFactory
    {
        IntegrationTask IIntegrationTaskFactory.CreateIntegrationTask(IEnumerable<int> keysToUpdate,
            DateTime currentTime)
        {
            return new IntegrationTask
            {
                State = IntegrationTaskStates.Initial,
                StartDate = currentTime
            };
        }
    }
}