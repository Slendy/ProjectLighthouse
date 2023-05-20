using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using StackExchange.Redis;

namespace LBPUnion.ProjectLighthouse.Types.Entities.Profile;

public class UserRoleEntity
{
    [Key]
    public int UserRoleId { get; set; }

    [ForeignKey(nameof(UserId))]
    public UserEntity User { get; set; }

    public int UserId { get; set; }

    [ForeignKey(nameof(RoleId))]
    public Role Role { get; set; }

    public int RoleId { get; set; }

}