using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LBPUnion.ProjectLighthouse.Configuration;
using LBPUnion.ProjectLighthouse.Database;
using LBPUnion.ProjectLighthouse.Helpers;
using LBPUnion.ProjectLighthouse.Services;
using LBPUnion.ProjectLighthouse.Types.Matchmaking.Rooms;
using LBPUnion.ProjectLighthouse.Types.Users;
using Redis.OM;

namespace LBPUnion.ProjectLighthouse.Tests.Helpers;

public static class IntegrationHelper
{
    private static readonly Lazy<bool> dbConnected = new(() =>
    {
        using DatabaseContext database = DatabaseContext.CreateNewInstance();
        return database.Database.CanConnect();
    });

    private static readonly Lazy<bool> redisConnected = new(PingRedis);

    private static bool PingRedis()
    {
        RedisConnectionProvider provider = new(ServerConfiguration.Instance.RedisConnectionString);
        RedisReply reply =  provider.Connection.Execute("PING");
        return reply.Error;
    }


    /// <summary>
    /// Resets the database to a clean state and returns a new DatabaseContext.
    /// </summary>
    /// <returns>A new fresh instance of DatabaseContext</returns>
    public static async Task<DatabaseContext> GetIntegrationDatabase()
    {
        if (!dbConnected.Value)
        {
            throw new Exception("Database is not connected.\n" +
                                "Please ensure that the database is running and that the connection string is correct.\n" +
                                $"Connection string: {ServerConfiguration.Instance.DbConnectionString}");
        }
        await using DatabaseContext database = DatabaseContext.CreateNewInstance();
        await database.Database.EnsureDeletedAsync();
        await database.Database.EnsureCreatedAsync();

        return DatabaseContext.CreateNewInstance();
    }

    public static async Task<RedisConnectionProvider> GetIntegrationRedis()
    {
        if (!redisConnected.Value)
        {
            throw new Exception("Redis is not connected\n" +
                                "Please ensure that Redis is running and that the connection string is correct.\n" +
                                $"Connection string: {ServerConfiguration.Instance.RedisConnectionString}");
        }
        RedisConnectionProvider provider = new(ServerConfiguration.Instance.RedisConnectionString);
        await provider.Connection.ExecuteAsync("FLUSHALL");
        await new IndexCreationService(provider,
            new List<Type>
            {
                typeof(Room),
                typeof(UserFriendData),
            }).StartAsync(new CancellationToken());
        return provider;
    }

    private static async Task ClearRooms()
    {
        await RoomHelper.Rooms.RemoveAllAsync();
    }

}