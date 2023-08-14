using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LBPUnion.ProjectLighthouse.Types.Entities.Matching;

public class RoomPlayerEntity
{
    [Key]
    public Guid RoomPlayerId { get; set; }

    public string Username { get; set; }

    public int? UserId { get; set; }

    public Guid RoomId { get; set; }

    [ForeignKey(nameof(RoomId))]
    public RoomEntity Room { get; set; }
}