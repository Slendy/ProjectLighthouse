using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using LBPUnion.ProjectLighthouse.Redis;

namespace LBPUnion.ProjectLighthouse.Tests.Types;

public class TestRedisRepository<T> : IRedisRepository<T>
{
    private readonly List<T> internalList;

    public TestRedisRepository()
    {
        this.internalList = new List<T>();
    }

    public void Save() { }

    public void Add(T item)
    {
        this.internalList.Add(item);
    }

    public void Remove(T item)
    {
        this.internalList.Remove(item);
    }

    public void Update(T item)
    {
        this.internalList.Remove(item);
        this.internalList.Add(item);
    }

    public IEnumerable<T> GetItems() => this.internalList.AsEnumerable();

    public T? FirstOrDefault(Expression<Func<T, bool>> expression) => this.internalList.FirstOrDefault(expression.Compile());

    public Task AddAsync(T item)
    {
        this.internalList.Add(item);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(T item)
    {
        this.internalList.Add(item);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(T item)
    {
        this.internalList.Add(item);
        return Task.CompletedTask;
    }

    public IAsyncEnumerable<T> GetItemsAsync() => this.internalList.ToAsyncEnumerable();

    public Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> expression) => Task.FromResult(this.internalList.FirstOrDefault(expression.Compile()));
}