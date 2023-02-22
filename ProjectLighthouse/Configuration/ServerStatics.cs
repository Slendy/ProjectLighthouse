#nullable enable
using System;
using System.Globalization;
using System.Linq;
using LBPUnion.ProjectLighthouse.Database;
using LBPUnion.ProjectLighthouse.Logging;
using LBPUnion.ProjectLighthouse.Types.Logging;
using LBPUnion.ProjectLighthouse.Types.Misc;
using Redis.OM;

namespace LBPUnion.ProjectLighthouse.Configuration;

public static class ServerStatics
{
    public const int PageSize = 20;

    public static bool DbConnected {
        get {
            try
            {
                using DatabaseContext db = new();
                return db.Database.CanConnect();
            }
            catch(Exception e)
            {
                Logger.Error(e.ToString(), LogArea.Database);
                return false;
            }
        }
    }

    public static bool RedisConnected
    {
        get
        {
            try
            {
                RedisConnectionProvider provider = new(ServerConfiguration.Instance.RedisConnectionString);
                string reply = provider.Connection.Execute("ping").ToString(CultureInfo.InvariantCulture);
                provider.Connection.Dispose();
                return reply == "PONG";
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }

    // FIXME: This needs to go at some point.
    public static bool IsUnitTesting => AppDomain.CurrentDomain.GetAssemblies().Any(assembly => assembly.FullName!.StartsWith("xunit"));

    public static bool IsDebug()
    {
        #if DEBUG
        return true;
        #else
        return false;
        #endif
    }

    /// <summary>
    /// The server type, determined on startup. Shouldn't be null unless very very early in startup.
    /// </summary>
    // The way of doing this is kinda weird, but it works.
    public static ServerType ServerType;
}