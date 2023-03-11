using LBPUnion.ProjectLighthouse.Database;
using LBPUnion.ProjectLighthouse.Servers.Website.Pages.Layouts;
using LBPUnion.ProjectLighthouse.Types.Entities.Profile;
using Microsoft.AspNetCore.Mvc;

namespace LBPUnion.ProjectLighthouse.Servers.Website.Pages;

public class EarthEditorPage : BaseLayout
{
    private readonly DatabaseContext database;

    public User ProfileUser { get; set; } = null!;

    public EarthEditorPage(DatabaseContext database) : base(database)
    {
        this.database = database;
    }

    public async Task<IActionResult> OnGet(int userId)
    {
        User? user = await this.database.Users.FindAsync(userId);
        if (user == null) return this.Redirect("~/");

        if (this.User == null) return this.Redirect("~/");

        if (this.User != this.ProfileUser && !this.User.IsModerator) return this.Redirect("~/");

        //TODO fetch all user slot locations and profile card location
        return this.Page();
    }
}