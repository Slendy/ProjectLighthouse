using LBPUnion.ProjectLighthouse.Database;
using LBPUnion.ProjectLighthouse.Servers.Website.Pages.Layouts;
using LBPUnion.ProjectLighthouse.Types.Roles;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LBPUnion.ProjectLighthouse.Servers.Website.Pages.Roles;

public class AddRolePage : BaseLayout
{
    public int TotalRoles; 
    public AddRolePage(DatabaseContext database) : base(database)
    { }

    public async Task<IActionResult> OnGet()
    {
        if (this.User == null) return this.Redirect("~/login");
        if (!this.User.HasPermission(Entitlements.ManageRoles)) return this.Redirect("~/");

        this.TotalRoles = await this.Database.Roles.CountAsync();

        return this.Page();
    }
}