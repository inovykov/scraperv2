using System;
using System.Collections.Generic;
using Shared.Models.Integration;

namespace IntegrationBl.Services
{
    public interface IIntegrationTaskFactory
    {
        IntegrationTask CreateIntegrationTask(IEnumerable<int> keysToUpdate, DateTime currentTime);
    }
}