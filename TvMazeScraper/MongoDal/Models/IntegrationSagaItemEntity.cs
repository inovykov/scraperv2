using System;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDal.Models
{
    public class IntegrationSagaItemEntity
    {
        [BsonId]
        public int Id { get; set; }
        
        public Guid SagaId { get; set; }
    }
}