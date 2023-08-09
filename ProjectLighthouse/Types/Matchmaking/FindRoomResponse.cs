using System.Collections.Generic;
using Newtonsoft.Json;

namespace LBPUnion.ProjectLighthouse.Types.Matchmaking;

public class FindRoomResponse : MatchResponse
{
    public List<RoomPlayerResponse> Players { get; set; }
    public List<List<int>> Slots { get; set; }
    public byte RoomState { get; set; }
    public byte HostMood { get; set; }
}

public class RoomPlayerResponse
{
    [JsonProperty("UserId")]
    public string Username { get; set; }

    [JsonProperty("matching_res")]
    public byte MatchingRes { get; set; }
}