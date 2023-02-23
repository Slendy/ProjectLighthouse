#nullable enable
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using LBPUnion.ProjectLighthouse.Logging;
using LBPUnion.ProjectLighthouse.Redis;
using LBPUnion.ProjectLighthouse.Types.Logging;
using LBPUnion.ProjectLighthouse.Types.Redis;

namespace LBPUnion.ProjectLighthouse.Types.Matchmaking.MatchCommands;

[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
public class UpdatePlayersInRoom : IMatchCommand
{
    public List<string>? Players { get; set; }
    public List<string>? Reservations { get; set; }

    public async Task<string?> ProcessCommand(RedisUser user, RedisRoom room, RoomRepository roomRepository, UserRepository userRepository)
    {
        if (this.Players == null) return null;
        if (this.Reservations == null) return null;

        if (this.Players.Contains(user.Username)) return null;
        if (user.UserId != room.RoomHostId) return null;

        List<int> roomMembers = new()
        {
            room.RoomHostId,
        };

        foreach (string username in this.Players.Where(username => username != user.Username))
        {
            RedisUser? roomMember = await userRepository.GetUser(username);
            if (roomMember == null)
            {
                roomMembers.Add(-1);
                Logger.Debug($"UpdatePlayersInRoom: Failed to fetch RedisUser for {username}", LogArea.Match);
                continue;
            }
            roomMembers.Add(roomMember.UserId);
        }

        room.RoomMembers = roomMembers.ToArray();
        await roomRepository.SaveAsync();

        return IMatchCommand.ValidCommand;
    }
}