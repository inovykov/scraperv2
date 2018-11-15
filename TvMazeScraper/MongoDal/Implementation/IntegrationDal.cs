using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDal.Configurations;
using MongoDal.Models;
using MongoDB.Driver;
using Shared.Interfaces;
using Shared.Models.Integration;

namespace MongoDal.Implementation
{
    public class IntegrationDal : MongoContextBase,  IIntegrationDal
    {
        private readonly IMapper _mapper;
        

        public IntegrationDal(MongoClient mongoClient, 
            IOptions<MongoConfig> mongoConfig, IMapper mapper)
            : base (mongoClient, mongoConfig)
        {
            _mapper = mapper;
            
        }

        public IMongoCollection<IntegrationTaskEntity> TasksCollection =>
            Database.GetCollection<IntegrationTaskEntity>(MongoConfig.TasksCollectionName);
        

        public async Task<IntegrationTask> GetTaskInProgressAsync(CancellationToken cancellationToken)
        {
            var integrationSagaEntity = await TasksCollection.Find(x => x.State == IntegrationTaskStates.InProgress).FirstOrDefaultAsync(cancellationToken);
            
            return _mapper.Map<IntegrationTask>(integrationSagaEntity);
        }

        public async Task<IntegrationTask> GetMostRecentCompletedTaskAsync(CancellationToken cancellationToken)
        {
            var sortExpr = Builders<IntegrationTaskEntity>.Sort.Descending(s => s.StartDate);

            var sagaEntity = await TasksCollection.Find(s => s.State == IntegrationTaskStates.Completed).Sort(sortExpr)
                .FirstOrDefaultAsync(cancellationToken);

            return _mapper.Map<IntegrationTask>(sagaEntity);
        }

        public async Task SaveIntegrationTaskAsync(IntegrationTask integrationTask,
            CancellationToken cancellationToken)
        {
            var integrationSagaEntity = _mapper.Map<IntegrationTaskEntity>(integrationTask);
            
            await TasksCollection.InsertOneAsync(integrationSagaEntity, null, cancellationToken);
        }

        public async Task SetIntegrationTaskStateAsync(Guid id, IntegrationTaskStates state,
            CancellationToken cancellationToken)
        {
            var filter = Builders<IntegrationTaskEntity>.Filter.Eq(s => s.Id, id);
            var update = Builders<IntegrationTaskEntity>.Update.Set(s => s.State, state);

            await TasksCollection.UpdateOneAsync(filter, update, null, cancellationToken);
        }
    }
}
