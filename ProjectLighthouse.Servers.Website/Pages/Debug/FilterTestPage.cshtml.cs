#nullable enable
#if DEBUG
using LBPUnion.ProjectLighthouse.Helpers;
#endif
using LBPUnion.ProjectLighthouse.Configuration;
using LBPUnion.ProjectLighthouse.Database;
using LBPUnion.ProjectLighthouse.Servers.Website.Pages.Layouts;
using Microsoft.AspNetCore.Mvc;

namespace LBPUnion.ProjectLighthouse.Servers.Website.Pages.Debug;

public class FilterTestPage : BaseLayout
{
    private readonly CensorConfiguration censorConfiguration;
    public FilterTestPage(DatabaseContext database, CensorConfiguration censorConfiguration) : base(database)
    {
        this.censorConfiguration = censorConfiguration;
    }

    public string? FilteredText;
    public string? Text;

    public IActionResult OnGet(string? text = null)
    {
        #if DEBUG
        if (text != null) this.FilteredText = CensorHelper.FilterMessage(this.censorConfiguration, text);
        this.Text = text;

        return this.Page();
        #else
        return this.NotFound();
        #endif
    }
}