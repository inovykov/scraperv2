using System;
using System.Collections.Generic;
using Shared.Models.Integration;

namespace IntegrationBl.Services
{
    public interface IIntegrationSagaFactory
    {
        IntegrationSagaExtended CreateIntegrationSaga(IEnumerable<int> keysToUpdate, DateTime currentTime);
    }
}