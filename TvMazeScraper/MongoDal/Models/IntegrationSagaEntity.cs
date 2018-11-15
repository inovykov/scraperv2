using System;
using MongoDB.Bson.Serialization.Attributes;
using Shared.Models.Integration;

namespace MongoDal.Models
{
    public class IntegrationSagaEntity
    {
        [BsonId]
        public Guid Id { get; set; }

        public SagaStates State { get; set; }

        public DateTime StartDate { get; set; }
    }
}
