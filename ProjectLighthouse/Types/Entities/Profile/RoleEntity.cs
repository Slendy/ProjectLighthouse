using System.ComponentModel.DataAnnotations;
using LBPUnion.ProjectLighthouse.Types.Roles;

namespace LBPUnion.ProjectLighthouse.Types.Entities.Profile;

public class RoleEntity
{
    [Key]
    public int RoleId { get; set; }

    public string Name { get; set; }

    public Entitlements Permissions { get; set; }

    public int Priority { get; set; }

    public bool DisplayOnProfile { get; set; }

    public long Color { get; set; }
}