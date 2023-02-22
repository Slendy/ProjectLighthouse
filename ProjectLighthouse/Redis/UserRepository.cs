#nullable enable
using System.Threading.Tasks;
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
        await ExtendUserSession(user);
    }

    public async Task ExtendUserSession(RedisUser user)
    {
        await this.provider.Connection.ExecuteAsync($"EXPIRE Users:{user.UserId} 3600");
    }

    public Task<RedisUser?> GetUser(int userId) => this.users.FindByIdAsync(userId.ToString());

    public Task<RedisUser?> GetUser(string username) => this.users.Where(u => u.Username == username).FirstOrDefaultAsync();



}