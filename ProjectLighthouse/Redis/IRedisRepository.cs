#nullable enable
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace LBPUnion.ProjectLighthouse.Redis;

public interface IRedisRepository<T>
{
    public void Save();
    public void Add(T item);
    public void Remove(T item);
    public void Update(T item);
    public IEnumerable<T> GetItems();
    public T? FirstOrDefault(Expression<Func<T, bool>> expression);

    public Task AddAsync(T item);
    public Task RemoveAsync(T item);
    public Task UpdateAsync(T item);
    public IAsyncEnumerable<T> GetItemsAsync();
    public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> expression);

}