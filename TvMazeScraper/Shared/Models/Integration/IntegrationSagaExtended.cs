using System.Collections.Generic;

namespace Shared.Models.Integration
{
    public class IntegrationSagaExtended : IntegrationSaga
    {
        public IList<IntegrationItem> IntegrationItems { get; set; }
    }
}