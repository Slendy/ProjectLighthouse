using LBPUnion.ProjectLighthouse.Configuration;
using LBPUnion.ProjectLighthouse.Database;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace LBPUnion.ProjectLighthouse.Tests;

public sealed class DatabaseFactAttribute : FactAttribute
{
    private static readonly object migrateLock = new();

    public DatabaseFactAttribute()
    {
        ServerConfiguration config = new ServerConfiguration
        {
            DbConnectionString = "server=127.0.0.1;uid=root;pwd=lighthouse;database=lighthouse"
        };
        using DatabaseContext? database = ServerStatics.DbConnected(config);
        if (database == null) this.Skip = "Database not available";
        else
            lock (migrateLock)
            {
                database.Database.Migrate();
            }
    }
}