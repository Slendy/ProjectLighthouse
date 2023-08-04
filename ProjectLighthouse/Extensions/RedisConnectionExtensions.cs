using System;
using System.Reflection;
using Redis.OM;
using StackExchange.Redis;

namespace LBPUnion.ProjectLighthouse.Extensions;

public static class RedisConnectionExtensions
{
    public static string[] SerializeTypeIndex(Type type)
    {
        Assembly redisAssembly = typeof(RedisConnectionProvider).Assembly;
        Type indexType = redisAssembly.GetType("Redis.OM.Modeling.RedisIndex");
        if (indexType == null) return null;
        MethodInfo methodInfo = indexType.GetMethod("SerializeIndex", BindingFlags.Static | BindingFlags.NonPublic);
        if (methodInfo == null) return null;
        object result = methodInfo.Invoke(null,
            new object[]
            {
                type,
            });
        return result as string[];
    }

    public static bool CloseRedisConnection(this RedisConnectionProvider provider)
    {
        FieldInfo muxerField = provider.GetType().GetField("_mux", BindingFlags.Instance | BindingFlags.NonPublic);
        if (muxerField == null) return false;

        if (muxerField.GetValue(provider) is not IConnectionMultiplexer multiplexer) return false;

        multiplexer.Close();
        return true;
    }
}