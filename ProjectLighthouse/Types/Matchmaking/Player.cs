using System;
using System.Text.Json.Serialization;

namespace LBPUnion.ProjectLighthouse.Types.Matchmaking;

[Serializable]
public class Player
{
    [JsonPropertyName("PlayerId")]
    public string Username { get; set; }

    [JsonPropertyName("matching_res")]
    public int MatchingRes { get; set; }
}