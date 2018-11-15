using System;

namespace Shared.Models.Integration
{
    public class IntegrationSaga
    {
        public Guid Id { get; set; }

        public SagaStates State { get; set; }

        public DateTime StartDate { get; set; }
    }
}
