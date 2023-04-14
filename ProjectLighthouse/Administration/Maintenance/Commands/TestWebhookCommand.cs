using System;
using System.Threading.Tasks;
using LBPUnion.ProjectLighthouse.Logging;
using LBPUnion.ProjectLighthouse.Types.Maintenance;
using LBPUnion.ProjectLighthouse.Types.Webhook;
using Microsoft.Extensions.DependencyInjection;

namespace LBPUnion.ProjectLighthouse.Administration.Maintenance.Commands;

public class TestWebhookCommand : ICommand
{
    public async Task Run(IServiceProvider serviceProvider, string[] args, Logger logger)
    {
        WebhookService webhookService = serviceProvider.GetRequiredService<WebhookService>();
        await webhookService.SendWebhook("Testing 123", "Someone is testing the Discord webhook from the admin panel.");
    }
    public string Name() => "Test Discord Webhook";
    public string[] Aliases()
        => new[]
        {
            "testWebhook", "testDiscordWebhook",
        };
    public string Arguments() => "";
    public int RequiredArgs() => 0;
}