using System;
using SSR.WebAPI.Models;
using SSR.WebAPI.Params;

namespace SSR.WebAPI.Interfaces
{
    public interface IAsyncRepository<TEntity, UEntityId> where TEntity : IIdEntity<UEntityId>
    {
        Task<long> CountAsync();
        Task<TEntity> GetByIdAsync(UEntityId id);
        Task<TEntity> CreateAsync(TEntity entity);
        Task<TEntity> UpdateAsync(TEntity model);
        Task DeleteAsync(TEntity model);
        Task DeleteWithIdAsync(UEntityId id);
        Task<List<TEntity>> GetAsync();
        Task<PagingModel<TEntity>> GetPagingAsync(PagingParam param);
    }
}

