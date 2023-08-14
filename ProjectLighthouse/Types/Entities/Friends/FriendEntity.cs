using System;
using System.ComponentModel.DataAnnotations;

namespace LBPUnion.ProjectLighthouse.Types.Entities.Friends;

public class FriendEntity
{
    [Key]
    public Guid FriendEntityId { get; set; }

    public int UserId { get; set; }

    public int FriendId { get; set; }
}