using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace LBPUnion.ProjectLighthouse.Servers.Website.Pages.Errors;

[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
[IgnoreAntiforgeryToken]
public class ErrorPage : PageModel
{
    public string? RequestId { get; set; }

    public IActionResult OnGet()
    {
        this.RequestId = this.HttpContext.TraceIdentifier;

        IExceptionHandlerPathFeature? exceptionHandlerPathFeature = this.HttpContext.Features.Get<IExceptionHandlerPathFeature>();

        if (exceptionHandlerPathFeature?.Error == null) return this.NotFound();

        Console.WriteLine(exceptionHandlerPathFeature.Path);
        Console.WriteLine(exceptionHandlerPathFeature.RouteValues);

        return this.Page();
    }
}