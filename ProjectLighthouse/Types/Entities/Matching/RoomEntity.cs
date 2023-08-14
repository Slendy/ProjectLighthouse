using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using LBPUnion.ProjectLighthouse.Types.Matchmaking.Rooms;
using LBPUnion.ProjectLighthouse.Types.Users;

namespace LBPUnion.ProjectLighthouse.Types.Entities.Matching;

public class RoomEntity
{
    [Key]
    public Guid RoomId { get; set; }

    public RoomState RoomState { get; set; }

    public GameVersion RoomVersion { get; set; }

    public Platform RoomPlatform { get; set; }

    public RoomMood RoomMood { get; set; }

    public DateTime LastUpdated { get; set; }

    public ICollection<RoomPlayerEntity> Users { get; set; }

    public ICollection<RoomPlayerExpirationEntity> BlockList { get; set; }
}