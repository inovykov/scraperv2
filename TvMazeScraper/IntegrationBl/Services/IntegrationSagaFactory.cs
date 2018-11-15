using System;
using System.Collections.Generic;
using System.Linq;
using Shared.Models.Integration;

namespace IntegrationBl.Services
{
    public class IntegrationSagaFactory : IIntegrationSagaFactory
    {
        public IntegrationSagaExtended CreateIntegrationSaga(IEnumerable<int> keysToUpdate, DateTime currentTime)
        {
            return new IntegrationSagaExtended
            {
                Id = Guid.NewGuid(),

                State = SagaStates.Initial,

                IntegrationItems = keysToUpdate.Select(k => new IntegrationItem(k))
                .ToList(),

                StartDate = currentTime

            };
        }
    }
}