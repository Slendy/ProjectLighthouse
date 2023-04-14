using LBPUnion.ProjectLighthouse.Configuration;
using LBPUnion.ProjectLighthouse.Database;
using LBPUnion.ProjectLighthouse.Servers.Website.Pages.Layouts;
using Microsoft.AspNetCore.Mvc;

namespace LBPUnion.ProjectLighthouse.Servers.Website.Pages.Debug;

public class VersionInfoPage : BaseLayout
{
    public VersionInfoPage(DatabaseContext database, ServerConfiguration serverConfiguration) : base(database, serverConfiguration)
    {}
    public IActionResult OnGet() => this.Page();
}