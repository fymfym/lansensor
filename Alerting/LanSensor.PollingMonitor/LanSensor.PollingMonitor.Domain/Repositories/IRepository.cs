using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LanSensor.PollingMonitor.Domain.Repositories
{
    public interface IRepository<TDomainEntity, TInfrastructureEntity>
        where TDomainEntity : class, IEntity
        where TInfrastructureEntity : class, IInfrastructureEntity
    {
        Task<TDomainEntity> GetByIdAsync(string id);
        Task<IEnumerable<TDomainEntity>> GetAllAsync();
        Task<TDomainEntity> CreateAsync(TDomainEntity entity);
        Task<TDomainEntity> UpdateAsync(TDomainEntity entity);
        Task DeleteAsync(string id);
    }
}
