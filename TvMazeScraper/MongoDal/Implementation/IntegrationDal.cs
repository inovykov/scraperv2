using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Options;
using MongoDal.Configurations;
using MongoDal.Models;
using MongoDB.Driver;
using Shared.Interfaces;
using Shared.Models.Integration;
using Shared.Services;

namespace MongoDal.Implementation
{
    public class IntegrationDal : MongoContextBase,  IIntegrationDal
    {
        private readonly IMapper _mapper;
        private readonly IRandomNumberService _randomNumberService;

        private readonly InsertManyOptions _insertManyOptions = new InsertManyOptions
        {
            BypassDocumentValidation = false,
            IsOrdered = false
        };

        public IntegrationDal(MongoClient mongoClient, 
            IOptions<MongoConfig> mongoConfig, IMapper mapper, IRandomNumberService randomNumberService)
            : base (mongoClient, mongoConfig)
        {
            _mapper = mapper;
            _randomNumberService = randomNumberService;
        }

        public IMongoCollection<IntegrationSagaEntity> SagasCollection =>
            Database.GetCollection<IntegrationSagaEntity>(MongoConfig.SagasCollectionName);

        public IMongoCollection<IntegrationSagaItemEntity> SagaItemsCollection =>
            Database.GetCollection<IntegrationSagaItemEntity>(MongoConfig.SagasItemsCollectionName);

        public async Task<IntegrationSaga> GetSagaInProgressAsync(CancellationToken cancellationToken)
        {
            var integrationSagaEntity = await SagasCollection.Find(x => x.State == SagaStates.InProgress).FirstOrDefaultAsync(cancellationToken);
            
            return _mapper.Map<IntegrationSaga>(integrationSagaEntity);
        }

        public async Task<IntegrationSaga> GetMostRecentCompletedSagaAsync(CancellationToken cancellationToken)
        {
            var sortExpr = Builders<IntegrationSagaEntity>.Sort.Descending(s => s.StartDate);

            var sagaEntity = await SagasCollection.Find(s => s.State == SagaStates.Completed).Sort(sortExpr).Limit(1)
                .FirstOrDefaultAsync(cancellationToken);

            return _mapper.Map<IntegrationSaga>(sagaEntity);
        }

        public async Task SaveSagaAsync(IntegrationSagaExtended integrationSaga, CancellationToken cancellationToken)
        {
            var integrationSagaEntity = _mapper.Map<IntegrationSagaEntity>(integrationSaga);

            var integrationSagaItems = _mapper.Map<IEnumerable<IntegrationSagaItemEntity>>(integrationSaga);

            await SagaItemsCollection.InsertManyAsync(integrationSagaItems, _insertManyOptions, cancellationToken);
            
            await SagasCollection.InsertOneAsync(integrationSagaEntity, null, cancellationToken);

            await SetSagaStateAsync(integrationSagaEntity.Id, SagaStates.InProgress, cancellationToken);
        }

        public async Task SetSagaStateAsync(Guid id, SagaStates state, CancellationToken cancellationToken)
        {
            var filter = Builders<IntegrationSagaEntity>.Filter.Eq(s => s.Id, id);
            var update = Builders<IntegrationSagaEntity>.Update.Set(s => s.State, state);

            await SagasCollection.UpdateOneAsync(filter, update, null, cancellationToken);
        }

        public async Task<IntegrationItem> GetSingleSagaItemBySagaIdAsync(Guid sagaId, CancellationToken cancellationToken)
        {
            var sagaItem = await SagaItemsCollection.Find(i => i.SagaId == sagaId).FirstOrDefaultAsync(cancellationToken);

            return _mapper.Map<IntegrationItem>(sagaItem);
        }

        public async Task<IntegrationItem> GetRandomSagaItemAsync(Guid sagaId, CancellationToken cancellationToken)
        {
            var filter = Builders<IntegrationSagaItemEntity>.Filter.Eq(i => i.SagaId, sagaId);

            // using deprecated method since it's much faster
            var count = await SagaItemsCollection.CountAsync(filter, null, cancellationToken);
            
            if (count == 0)
            {
                return null; // all saga items processed
            }

            var randomSkipSize = _randomNumberService.GetRandom(Convert.ToInt32(count - 1));

            var sagaItem = await SagaItemsCollection.Find(s => s.SagaId == sagaId).Skip(randomSkipSize)
                .FirstOrDefaultAsync(cancellationToken);
            
            return _mapper.Map<IntegrationItem>(sagaItem);
        }

        public async Task DeleteSagaItemByIdAsync(int id, CancellationToken cancellationToken)
        {
            await SagaItemsCollection.DeleteOneAsync(i => i.Id == id, cancellationToken);
        }
    }
}
