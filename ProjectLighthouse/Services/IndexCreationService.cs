using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Redis.OM;

namespace LBPUnion.ProjectLighthouse.Services;

public class IndexCreationService : IHostedService
{
    private readonly RedisConnectionProvider provider;
    private readonly List<Type> entityTypes;

    public IndexCreationService(RedisConnectionProvider provider, List<Type> entityTypes)
    {
        this.provider = provider;
        this.entityTypes = entityTypes;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        foreach (Type type in this.entityTypes)
        {
            if (await this.provider.Connection.GetIndexInfoAsync(type) == null)
            {
                await this.provider.Connection.CreateIndexAsync(type);
            }
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}