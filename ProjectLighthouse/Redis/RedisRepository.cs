using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Redis.OM.Searching;

namespace LBPUnion.ProjectLighthouse.Redis;

public class RedisRepository<T> : IRedisRepository<T>
{
    private readonly RedisCollection<T> collection;

    public RedisRepository(RedisCollection<T> collection)
    {
        this.collection = collection;
    }

    public void Save()
    {
        this.collection.Save();
    }

    public void Add(T item)
    {
        this.collection.Insert(item);
    }

    public void Remove(T item)
    {
        this.collection.Delete(item);
    }

    public void Update(T item)
    {
        this.collection.Update(item);
    }

    public IEnumerable<T> GetItems() => this.collection.AsEnumerable();

    public T FirstOrDefault(Expression<Func<T, bool>> expression) => this.collection.FirstOrDefault(expression);

    public Task AddAsync(T item) => this.collection.InsertAsync(item);

    public Task RemoveAsync(T item) => this.collection.DeleteAsync(item);

    public Task UpdateAsync(T item) => this.collection.UpdateAsync(item);

    public IAsyncEnumerable<T> GetItemsAsync() => this.collection.AsAsyncEnumerable();

    public Task<T> FirstOrDefaultAsync(Expression<Func<T, bool>> expression) => this.collection.FirstOrDefaultAsync(expression);
}