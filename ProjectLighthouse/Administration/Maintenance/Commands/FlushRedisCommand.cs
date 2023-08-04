using System;
using System.Threading.Tasks;
using LBPUnion.ProjectLighthouse.Logging;
using LBPUnion.ProjectLighthouse.Types.Maintenance;
using Microsoft.Extensions.DependencyInjection;
using Redis.OM.Contracts;

namespace LBPUnion.ProjectLighthouse.Administration.Maintenance.Commands;

public class FlushRedisCommand : ICommand
{
    public string Name() => "Flush Redis";
    public string[] Aliases() =>
        new[]
        {
            "flush", "flush-redis",
        };
    public string Arguments() => "";
    public int RequiredArgs() => 0;

    public async Task Run(IServiceProvider provider, string[] args, Logger logger)
    {
        IRedisConnectionProvider redis = provider.GetRequiredService<IRedisConnectionProvider>();
        await redis.Connection.ExecuteAsync("FLUSHALL");
    } 

}