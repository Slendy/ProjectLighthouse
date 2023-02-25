#nullable enable
using System;
using System.Threading.Tasks;
using LBPUnion.ProjectLighthouse.Types.Entities.Token;
using LBPUnion.ProjectLighthouse.Types.Redis;
using Redis.OM;
using Redis.OM.Searching;

namespace LBPUnion.ProjectLighthouse.Redis;

public class UserRepository
{
    private readonly RedisCollection<RedisUser> users;
    private readonly RedisConnectionProvider provider;

    public UserRepository(RedisConnectionProvider provider)
    {
        this.provider = provider;
        this.users = (RedisCollection<RedisUser>)provider.RedisCollection<RedisUser>();
    }

    public async Task AddUser(RedisUser user)
    {
        await this.users.InsertAsync(user);
        await this.ExtendUserSession(user);
    }

    public async Task<RedisUser> CreateUser(GameToken token, string username)
    {
        RedisUser redisUser = new()
        {
            BlockedList = Array.Empty<string>(),
            FriendList = Array.Empty<string>(),
            Rooms = Array.Empty<Ulid>(),
            UserId = token.UserId,
            Username = username,
        }; 
        await this.AddUser(redisUser);
        return redisUser;
    }

    public Task DeleteUser(RedisUser user) => this.users.DeleteAsync(user);

    public ValueTask SaveAsync() => this.users.SaveAsync();

    public async Task ExtendUserSession(RedisUser user)
    {
        await this.provider.Connection.ExecuteAsync("EXPIRE", $"User:{user.UserId}", "3600");
    }

    public Task<bool> UserExists(int userId) => this.users.AnyAsync(u => u.UserId == userId);

    public Task<RedisUser?> GetUser(int userId) => this.users.FindByIdAsync(userId.ToString());

    public Task<RedisUser?> GetUser(string username) => this.users.Where(u => u.Username == username).FirstOrDefaultAsync();

    public Task UpdateUser(RedisUser user) => this.users.UpdateAsync(user);

}