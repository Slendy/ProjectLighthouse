using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LBPUnion.ProjectLighthouse.Types.Entities.Matching;

public class RoomPlayerExpirationEntity
{
    [Key]
    public Guid RoomPlayerId { get; set; }

    public Guid RoomId { get; set; }

    [ForeignKey(nameof(RoomId))]
    public RoomEntity Room { get; set; } 

    public DateTime Expiration { get; set; }
}