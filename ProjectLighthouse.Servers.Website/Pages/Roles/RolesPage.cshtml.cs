using LBPUnion.ProjectLighthouse.Database;
using LBPUnion.ProjectLighthouse.Servers.Website.Pages.Layouts;
using LBPUnion.ProjectLighthouse.Types.Entities.Profile;
using LBPUnion.ProjectLighthouse.Types.Roles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LBPUnion.ProjectLighthouse.Servers.Website.Pages.Roles;

public class RolesPage : BaseLayout
{
    public List<RoleEntity> Roles = new();

    public RolesPage(DatabaseContext database) : base(database)
    { }

    public async Task<IActionResult> OnGet()
    {
        if (this.User == null) return this.Redirect("~/login");
        if (!this.User.HasPermission(Entitlements.ManageRoles)) return this.Redirect("~/");

        this.Roles = await this.Database.Roles.OrderByDescending(r => r.Priority).ToListAsync();

        return this.Page();
    }
}