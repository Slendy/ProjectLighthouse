using LBPUnion.ProjectLighthouse.Types.Entities.Friends;
using LBPUnion.ProjectLighthouse.Types.Entities.Matching;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace LBPUnion.ProjectLighthouse.Database;

public class MemoryContext : DbContext
{
    public MemoryContext(DbContextOptions<MemoryContext> options) : base(options)
    { }

    public DbSet<FriendEntity> Friends => this.Set<FriendEntity>();
    public DbSet<RoomEntity> Rooms => this.Set<RoomEntity>();
    public DbSet<RoomPlayerEntity> RoomPlayers => this.Set<RoomPlayerEntity>();
    public DbSet<RoomPlayerExpirationEntity> RoomPlayerExpirations => this.Set<RoomPlayerExpirationEntity>();

    public static MemoryContext CreateNewContext()
    {
        DbContextOptionsBuilder<MemoryContext> options = new();
        SqliteConnection connection = new("DataSource=Lighthouse;Mode=Memory;Cache=Shared");
        connection.Open();
        options.UseSqlite(connection);
        return new MemoryContext(options.Options);
    }
}