using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using LanSensor.PollingMonitor.Domain.Repositories;
using MongoDB.Driver;

namespace LanSensor.PollingMonitor.Infrastructure.DeviceState.MongoDb
{
    public abstract class MongoDbRepository<TDomainEntity, TInfrastructureEntity> : IRepository<TDomainEntity, TInfrastructureEntity>
        where TDomainEntity : class, IEntity
        where TInfrastructureEntity : class, IInfrastructureEntity
    {
        private readonly string _databaseId;
        private readonly string _collectionName;
        private readonly IMapper _mapper;

        private readonly IMongoClient _client;

        protected MongoDbRepository(
            string databaseId,
            string collectionName,
            IServiceConfiguration apiConfiguration,
            IMapper mapper)
        {
            _databaseId = databaseId ?? throw new ArgumentNullException(nameof(databaseId));
            _collectionName = collectionName;
            _mapper = mapper;
            _client = new MongoClient(apiConfiguration.ApplicationConfiguration.MongoDbConfiguration.ConnectionString);
        }

        public async Task<TDomainEntity> GetByIdAsync(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            var entity = await GetByIdInternalAsync(id);
            if (entity == null) return null;

            var result = _mapper.Map<TDomainEntity>(entity);
            result.EntityId = entity.EntityId;

            return result;
        }

        private async Task<TInfrastructureEntity> GetByIdInternalAsync(string id)
        {
            var collection = GetCollection();
            var result = await collection.FindAsync(GetIdFilter(id));
            return result.ToList().SingleOrDefault();
        }

        public async Task<IEnumerable<TDomainEntity>> GetBySearchAsync(FilterDefinition<TInfrastructureEntity> filter)
        {
            var collection = GetCollection();
            var result = await collection.FindAsync<TInfrastructureEntity>(filter);
            return result.ToList().Select(x => _mapper.Map<TDomainEntity>(x));
        }

        public async Task<IEnumerable<TDomainEntity>> GetAllAsync()
        {
            var result = new List<TDomainEntity>();
            foreach (var entity in await GetAllInternalAsync())
            {
                var e = _mapper.Map<TDomainEntity>(entity);
                e.EntityId = entity.EntityId;
                result.Add(e);
            }

            return result;
        }

        private async Task<IEnumerable<TInfrastructureEntity>> GetAllInternalAsync()
        {
            var collection = GetCollection();
            var result = await collection.FindAsync(GetEmptyFilter());
            return result.ToList();
        }

        public async Task<TDomainEntity> CreateAsync(TDomainEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var saveEntity = _mapper.Map<TInfrastructureEntity>(entity);
            var result = await CreateInternalAsync(saveEntity);
            var saved = _mapper.Map<TDomainEntity>(result);
            saved.EntityId = result.EntityId;
            return saved;
        }

        private async Task<TInfrastructureEntity> CreateInternalAsync(TInfrastructureEntity entity)
        {
            var collection = GetCollection();
            await collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task<TDomainEntity> UpdateAsync(TDomainEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            var updateEntity = _mapper.Map<TInfrastructureEntity>(entity);
            updateEntity.EntityId = entity.EntityId;

            var res = await UpdateInternalAsync(updateEntity);
            var updated = _mapper.Map<TDomainEntity>(res);
            updated.EntityId = entity.EntityId;
            return updated;
        }

        private async Task<TInfrastructureEntity> UpdateInternalAsync(TInfrastructureEntity entity)
        {
            var t = await GetByIdInternalAsync(entity.EntityId);

            var collection = GetCollection();
            var filter = GetIdFilter(entity.EntityId);
            entity.Id = t.Id;
            await collection.ReplaceOneAsync(filter, entity);
            return entity;
        }

        public Task DeleteAsync(string id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            return DeleteInternalAsync(id);
        }

        private async Task DeleteInternalAsync(string id)
        {
            var filter = GetIdFilter(id);
            await GetCollection().FindOneAndDeleteAsync(filter);
        }

        protected IMongoCollection<TInfrastructureEntity> GetCollection()
        {
            var db = _client.GetDatabase(_databaseId);
            return db.GetCollection<TInfrastructureEntity>(_collectionName);
        }

        private static FilterDefinition<TInfrastructureEntity> GetIdFilter(string id)
        {
            return Builders<TInfrastructureEntity>.Filter.Eq("EntityId", id);
        }

        private static FilterDefinition<TInfrastructureEntity> GetEmptyFilter()
        {
            return Builders<TInfrastructureEntity>.Filter.Empty;
        }
    }
}
