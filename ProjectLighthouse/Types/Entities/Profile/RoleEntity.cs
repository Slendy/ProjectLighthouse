using System.ComponentModel.DataAnnotations;
using LBPUnion.ProjectLighthouse.Types.Roles;

namespace LBPUnion.ProjectLighthouse.Types.Entities.Profile;

public class RoleEntity
{
    [Key]
    public int RoleId { get; set; }

    /// <summary>
    /// The name that will be displayed
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The permissions associated with a role
    /// </summary>
    public Entitlements Permissions { get; set; }

    /// <summary>
    /// A number that is used to sort roles and define permission hierarchy for users with multiple roles
    /// <para>
    ///     Highest priority is 0, the higher the number the lower the priority 
    /// </para>
    /// </summary>
    public uint Priority { get; set; }

    /// <summary>
    /// Whether the role should be displayed on user's profiles
    /// <para>
    ///     Useful for designing shadow-ban roles
    /// </para>
    /// </summary>
    public bool DisplayOnProfile { get; set; }

    /// <summary>
    /// An integer representation of an HTML hex color code
    /// </summary>
    public int Color { get; set; }
}