using System.Net;
using System.Net.Mail;
using LBPUnion.ProjectLighthouse.Configuration;
using LBPUnion.ProjectLighthouse.Types.Mail;

namespace LBPUnion.ProjectLighthouse.Mail;

public class SmtpMailSender : IMailSender
{
    private readonly ServerConfiguration serverConfiguration;

    public SmtpMailSender(ServerConfiguration serverConfiguration)
    {
        this.serverConfiguration = serverConfiguration;
    }

    public async void SendEmail(MailMessage message)
    {
        using SmtpClient client = new(this.serverConfiguration.Mail.Host, this.serverConfiguration.Mail.Port)
        {
            EnableSsl = this.serverConfiguration.Mail.UseSSL,
            Credentials = new NetworkCredential(this.serverConfiguration.Mail.Username, this.serverConfiguration.Mail.Password),
        };
        await client.SendMailAsync(message);
    }
}