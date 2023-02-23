using System.Threading;
using System.Threading.Tasks;
using LBPUnion.ProjectLighthouse.Extensions;
using LBPUnion.ProjectLighthouse.Types.Redis;
using Microsoft.Extensions.Hosting;
using Redis.OM;

namespace LBPUnion.ProjectLighthouse.Redis;

public class RedisStartupService : IHostedService
{
    private readonly RedisConnectionProvider provider;

    public RedisStartupService(RedisConnectionProvider provider)
    {
        this.provider = provider;
    }

    // public async Task StartAsync(CancellationToken cancellationToken)
    // {
    //     await _provider.Connection.CreateIndexAsync(typeof(Person));
    // }
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await this.provider.Connection.RecreateIndexAsync(typeof(RedisUser));
        await this.provider.Connection.RecreateIndexAsync(typeof(RedisRoom));
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}