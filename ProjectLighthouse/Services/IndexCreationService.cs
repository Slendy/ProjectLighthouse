using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LBPUnion.ProjectLighthouse.Extensions;
using LBPUnion.ProjectLighthouse.Helpers;
using LBPUnion.ProjectLighthouse.Logging;
using LBPUnion.ProjectLighthouse.Types.Logging;
using LBPUnion.ProjectLighthouse.Types.Matchmaking.Rooms;
using LBPUnion.ProjectLighthouse.Types.Users;
using Microsoft.Extensions.Hosting;
using Redis.OM;

namespace LBPUnion.ProjectLighthouse.Services;

public class IndexCreationService : IHostedService
{
    private readonly RedisConnectionProvider provider;

    public IndexCreationService(RedisConnectionProvider provider)
    {
        this.provider = provider;
    }

    private static string CalculateIndexHash(Type type)
    {
        string[] serializedIndex = RedisConnectionExtensions.SerializeTypeIndex(type);
        string concatenatedIndex = string.Join("", serializedIndex);
        return CryptoHelper.Sha1Hash(Encoding.UTF8.GetBytes(concatenatedIndex));
    }

    private async Task CreateIndex(Type type)
    {
        Logger.Debug("Created index for " + type.Name, LogArea.Redis);
        string indexHash = CalculateIndexHash(type);

        string indexHashKey = $"{type.Name.ToLower()}-hash";

        await this.provider.Connection.ExecuteAsync("SET", indexHashKey, indexHash);
        await this.provider.Connection.CreateIndexAsync(type);
    }

    private async Task CreateIndexIfNull(Type type)
    {
        string newIndexHash = CalculateIndexHash(type);

        string hashKey = $"{type.Name.ToLower()}-hash";

        RedisIndexInfo indexInfo = await this.provider.Connection.GetIndexInfoAsync(type);

        string currentIndexHash = await this.provider.Connection.ExecuteAsync("GET", hashKey);
        if (currentIndexHash == null || indexInfo == null)
        {
            Logger.Debug("Index does not exist for " + type.Name, LogArea.Redis);
            await this.CreateIndex(type);
            return;
        }
        
        if (currentIndexHash == newIndexHash) return;

        Logger.Debug("Index has changed for " + type.Name, LogArea.Redis);

        // If the index hash has changed we should probably drop all existing records to prevent conflicts
        this.provider.Connection.DropIndexAndAssociatedRecords(type);
        await this.CreateIndex(type);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await this.CreateIndexIfNull(typeof(UserFriendData));
        await this.CreateIndexIfNull(typeof(Room));
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}