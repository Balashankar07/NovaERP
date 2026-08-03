using NovaERP.Application.Common.Models;
﻿using NovaERP.Domain.Common;

namespace NovaERP.Application.Interfaces.Repositories
{
    public interface IRepository<T>
        where T : BaseEntity
    {
        Task<T?> GetByIdAsync(Guid id);

        Task<PagedResult<T>> GetAllAsync(int pageNumber = 1, int pageSize = 10, string? search = null, string? sortBy = null, string? sortOrder = null);

        Task AddAsync(T entity);

        void Update(T entity);

        void Delete(T entity);
    }
}