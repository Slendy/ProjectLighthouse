using LBPUnion.ProjectLighthouse.Database;
using LBPUnion.ProjectLighthouse.Servers.Website.Pages.Layouts;
using LBPUnion.ProjectLighthouse.Servers.Website.Types;
using LBPUnion.ProjectLighthouse.Types.Entities.Level;
using LBPUnion.ProjectLighthouse.Types.Entities.Profile;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace LBPUnion.ProjectLighthouse.Servers.Website.Pages;

public class EarthEditorPage : BaseLayout
{
    private readonly DatabaseContext database;

    public User? ProfileUser { get; set; }

    public UserEarth UserEarth { get; set; } = null!;

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

        this.ProfileUser = await this.database.Users.FindAsync(userId);

        if (this.ProfileUser == null) return this.Redirect("~/");

        this.UserEarth = new UserEarth();

        this.UserEarth.AddUserLocation(this.ProfileUser.Location);

        foreach(Slot s in await this.database.Slots.Where(s => s.CreatorId == this.ProfileUser.UserId).ToListAsync())
        {
            Console.WriteLine($"{s.Name} - {s.Location.X}, {s.Location.Y} - {s.GameVersion} - {s.CreatorId}");
            this.UserEarth.AddLevelLocation(s.Location);
        }

        //TODO fetch all user slot locations and profile card location
        return this.Page();
    }
}