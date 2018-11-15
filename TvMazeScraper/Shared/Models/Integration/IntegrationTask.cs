using System;

namespace Shared.Models.Integration
{
    public class IntegrationTask
    {
        public Guid Id { get; set; }

        public IntegrationTaskStates State { get; set; }

        public DateTime StartDate { get; set; }
    }
}
