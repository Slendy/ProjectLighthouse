#nullable enable
using LBPUnion.ProjectLighthouse.Configuration;
using LBPUnion.ProjectLighthouse.Database;
using LBPUnion.ProjectLighthouse.Helpers;
using LBPUnion.ProjectLighthouse.Servers.Website.Pages.Layouts;
using LBPUnion.ProjectLighthouse.Types.Entities.Profile;
using LBPUnion.ProjectLighthouse.Types.Mail;
using Microsoft.AspNetCore.Mvc;

namespace LBPUnion.ProjectLighthouse.Servers.Website.Pages.Email;

public class SendVerificationEmailPage : BaseLayout
{
    private readonly ServerConfiguration serverConfiguration;
    private readonly IMailService mailService;
    public SendVerificationEmailPage(DatabaseContext database, ServerConfiguration serverConfiguration, IMailService mailService) : base(database)
    {
        this.serverConfiguration = serverConfiguration;
        this.mailService = mailService;
    }

    public bool Success { get; set; }

    public async Task<IActionResult> OnGet()
    {
        if (!this.serverConfiguration.Mail.MailEnabled) return this.NotFound();

        UserEntity? user = this.Database.UserFromWebRequest(this.Request);
        if (user == null) return this.Redirect("/login");

        if (user.EmailAddressVerified) return this.Redirect("/");

        this.Success = await SMTPHelper.SendVerificationEmail(this.Database, this.mailService, this.serverConfiguration, user);

        return this.Page();
    }
}