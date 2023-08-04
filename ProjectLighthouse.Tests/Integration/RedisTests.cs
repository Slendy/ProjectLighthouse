using System;
using System.Linq;
using LBPUnion.ProjectLighthouse.Configuration;
using LBPUnion.ProjectLighthouse.Extensions;
using LBPUnion.ProjectLighthouse.Types.Matchmaking.Rooms;
using LBPUnion.ProjectLighthouse.Types.Users;
using Redis.OM;
using Xunit;

namespace LBPUnion.ProjectLighthouse.Tests.Integration;

/// <summary>
/// Ensures functionality of unstable functions that use reflection
/// to fetch private fields or functions
/// </summary>
[Trait("Category", "Integration")]
public class RedisTests
{
    [Fact]
    public void CloseRedisConnectionShouldClose()
    {
        RedisConnectionProvider provider = new(ServerConfiguration.Instance.RedisConnectionString);
        Assert.True(provider.CloseRedisConnection());
        Exception e = Assert.Throws<Exception>(() =>
        {
            provider.Connection.Execute("PING");
        });
        Assert.IsType<ObjectDisposedException>(e.InnerException);
    }

    [Fact]
    public void SerializeIndexOnRoomShouldSucceed()
    {
        string[] serialized = RedisConnectionExtensions.SerializeTypeIndex(typeof(Room));
        Assert.NotEmpty(serialized);
        Assert.Equal("room-idx", serialized.First());
    }

    [Fact]
    public void SerializeIndexOnFriendDataShouldSucceed()
    {
        string[] serialized = RedisConnectionExtensions.SerializeTypeIndex(typeof(UserFriendData));
        Assert.NotEmpty(serialized);
        Assert.Equal("userfrienddata-idx", serialized.First());
    }
}