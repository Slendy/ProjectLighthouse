#nullable enable
using System.Collections.Generic;
using LBPUnion.ProjectLighthouse.Types.Matchmaking.Rooms;
using Newtonsoft.Json;

namespace LBPUnion.ProjectLighthouse.Types.Matchmaking.MatchCommands;

public class RoomCommandData
{
    public string? Player { get; set; }

    public List<string>? Players { get; set; }

    public List<string>? Reservations { get; set; }

    [JsonProperty("NAT")]
    public List<NatType>? NatType { get; set; }

    public List<int>? Slot { get; set; }

    // LBP has two of the same thing sometimes, have both properties to handle both cases
    [JsonProperty]
    private List<int>? Slots
    {
        set => this.Slot = value;
    }

    public RoomState? RoomState { get; set; }

    public byte? Mood { get; set; }

    private byte? HostMood
    {
        set => this.Mood = value;
    }

    public bool? PassedNoJoinPoint { get; set; }

    [JsonProperty("Location")]
    public List<string>? Locations { get; set; }

    public byte? Language { get; set; }

    public int? BuildVersion { get; set; }

    public string? Search { get; set; }
}