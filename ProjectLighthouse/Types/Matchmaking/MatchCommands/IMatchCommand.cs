#nullable enable
using System.Threading.Tasks;
using LBPUnion.ProjectLighthouse.Redis;
using LBPUnion.ProjectLighthouse.Types.Redis;

namespace LBPUnion.ProjectLighthouse.Types.Matchmaking.MatchCommands;

public interface IMatchCommand
{
    public Task<string?> ProcessCommand(RedisUser user, RedisRoom room, RoomRepository roomRepository, UserRepository userRepository);
    public const string ValidCommand = "[{\"StatusCode\":200}]";

}