using System;
using MongoDB.Bson.Serialization.Attributes;
using Shared.Models.Integration;

namespace MongoDal.Models
{
    public class IntegrationTaskEntity
    {
        [BsonId]
        public Guid Id { get; set; }

        public IntegrationTaskStates State { get; set; }

        public DateTime StartDate { get; set; }
    }
}
