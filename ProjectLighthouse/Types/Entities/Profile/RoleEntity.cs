using LBPUnion.ProjectLighthouse.Types.Roles;

namespace LBPUnion.ProjectLighthouse.Types.Entities.Profile;

public class RoleEntity
{
    public int RoleId { get; set; }

    public string Name { get; set; }

    public Entitlements Permissions { get; set; }

    public bool DisplayOnProfile { get; set; }

}