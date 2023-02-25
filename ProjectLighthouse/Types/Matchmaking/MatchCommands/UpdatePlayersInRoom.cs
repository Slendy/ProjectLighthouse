#nullable enable
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using LBPUnion.ProjectLighthouse.Logging;
using LBPUnion.ProjectLighthouse.Redis;
using LBPUnion.ProjectLighthouse.Types.Entities.Token;
using LBPUnion.ProjectLighthouse.Types.Logging;
using LBPUnion.ProjectLighthouse.Types.Redis;

namespace LBPUnion.ProjectLighthouse.Types.Matchmaking.MatchCommands;

[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
public class UpdatePlayersInRoom : IMatchCommand
{
    public List<string>? Players { get; set; }
    public List<string>? Reservations { get; set; }

    public async Task<string?> ProcessCommand
    (
        GameToken token,
        RedisUser user,
        RedisRoom room,
        RoomRepository roomRepository,
        UserRepository userRepository
    )
    {
        if (this.Players == null) return null;
        if (this.Reservations == null) return null;

        if (!this.Players.Contains(user.Username)) return null;
        if (user.UserId != room.RoomHostId) return null;

        // TODO are reservations related to how many local players vs remote players are in the lobby?

        List<int> roomMembers = new();

        // If a user has local players in their room, then there will be duplicates of their name in the request
        foreach (string username in this.Players)
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
        Logger.Debug($"UpdatePlayersInRoom: Updated players for room {room.Id}: {string.Join(",", room.RoomMembers)}", LogArea.Match);
        await roomRepository.SaveAsync();

        return IMatchCommand.ValidCommand;
    }
}