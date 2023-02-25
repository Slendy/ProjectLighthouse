#nullable enable
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security.Cryptography;
using System.Text.Json;
using System.Threading.Tasks;
using LBPUnion.ProjectLighthouse.Logging;
using LBPUnion.ProjectLighthouse.Redis;
using LBPUnion.ProjectLighthouse.Types.Entities.Token;
using LBPUnion.ProjectLighthouse.Types.Levels;
using LBPUnion.ProjectLighthouse.Types.Logging;
using LBPUnion.ProjectLighthouse.Types.Matchmaking.Rooms;
using LBPUnion.ProjectLighthouse.Types.Redis;

namespace LBPUnion.ProjectLighthouse.Types.Matchmaking.MatchCommands;

// Schema is the EXACT SAME as CreateRoom (but cant be a subclass here), so see comments there for details
[SuppressMessage("ReSharper", "CollectionNeverUpdated.Global")]
public class FindBestRoom : IMatchCommand
{
    public int BuildVersion;
    public int HostMood;
    public int Language;
    public List<int>? Location;

    public List<int>? NAT;
    public int PassedNoJoinPoint;
    public RoomState? RoomState;
    public string? Search;
    public List<string>? Players { get; set; }

    public List<string>? Reservations { get; set; }
    public List<List<int>>? Slots { get; set; }

    public async Task<string?> ProcessCommand
    (
        GameToken token,
        RedisUser user,
        RedisRoom room,
        RoomRepository roomRepository,
        UserRepository userRepository
    )
    {
        if (user.UserId != room.RoomHostId) return null;
        if (this.Players == null) return null;
        if (this.RoomState == null) return null;
        if (this.Slots == null) return null;

        if (room.RoomBuildVersion == -1) room.RoomBuildVersion = this.BuildVersion;

        RoomSlot slot = new()
        {
            SlotType = (SlotType)this.Slots[0][0],
            SlotId = this.Slots[0][1],
        };

        room.RoomState = (RoomState)this.RoomState;
        room.RoomSlot = slot;

        await roomRepository.SaveAsync();

        List<RedisRoom> matchingRooms = (List<RedisRoom>)await roomRepository.FindMatchesForToken(token, room).ToListAsync();

        Shuffle(matchingRooms);

        foreach (RedisRoom r in matchingRooms)
        {
            bool allPlayersValid = r.RoomMembers.All(m => r.MemberLocations.ContainsKey(m));
            if (!allPlayersValid) continue;

            FindBestRoomResponse response = new()
            {
                RoomId = r.Id,
                Players = new List<Player>(),
                Locations = new List<string>(),
            };
            foreach (string uid in r.RoomMembers)
            {

                Player player = new()
                {
                    Username = (await userRepository.GetUser(uid))?.Username,
                    MatchingRes = 0,
                };
                response.Players.Add(player);

                response.Locations.Add(r.MemberLocations[uid]);
            }
            response.Players.Add(new Player
            {
                Username = user.Username,
                MatchingRes = 1,
            });

            response.Slots = new List<List<int>>
            {
                new()
                {
                    (int)r.RoomSlot.SlotType,
                    r.RoomSlot.SlotId,
                },
            };

            Logger.Success($"Found a room (id: {r.Id}) for user {user.Username} (id: {user.UserId})", LogArea.Match);
            string serialized = JsonSerializer.Serialize(response, typeof(FindBestRoomResponse));
            return $"[{IMatchCommand.ValidCommand},{serialized}]";
        }

        Logger.Error($"Failed to find a room for user {user.Username} (id: {user.UserId})", LogArea.Match);
        return null;
    }

    // https://stackoverflow.com/a/375361
    private static void Shuffle(IList<RedisRoom> rooms)
    {
        int n = rooms.Count; // The number of items left to shuffle (loop invariant).
        while (n > 1)
        {
            int k = RandomNumberGenerator.GetInt32(n); // 0 <= k < n.
            n--; // n is now the last pertinent index;
            (rooms[n], rooms[k]) = (rooms[k], rooms[n]);
        }
    }

}