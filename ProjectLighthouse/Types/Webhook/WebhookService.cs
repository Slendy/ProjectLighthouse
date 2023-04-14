using System;
using System.Threading.Tasks;
using Discord;
using Discord.Webhook;
using LBPUnion.ProjectLighthouse.Configuration;
using SixLabors.ImageSharp.PixelFormats;
using Color = SixLabors.ImageSharp.Color;
using DiscordColor = Discord.Color;

namespace LBPUnion.ProjectLighthouse.Types.Webhook;

public class WebhookService
{
    private readonly DiscordConfiguration discordConfiguration;

    private readonly DiscordWebhookClient publicClient;

    private readonly DiscordWebhookClient moderationClient;

    private readonly DiscordWebhookClient registrationClient;

    public WebhookService(DiscordConfiguration discordConfiguration)
    {
        this.discordConfiguration = discordConfiguration;
        if (!this.discordConfiguration.DiscordIntegrationEnabled) return;

        if(!string.IsNullOrWhiteSpace(discordConfiguration.PublicUrl))
            this.publicClient = new DiscordWebhookClient(discordConfiguration.PublicUrl);
        if (!string.IsNullOrWhiteSpace(discordConfiguration.ModerationUrl))
            this.moderationClient = new DiscordWebhookClient(discordConfiguration.ModerationUrl);
        if (!string.IsNullOrWhiteSpace(discordConfiguration.RegistrationUrl))
            this.registrationClient = new DiscordWebhookClient(discordConfiguration.RegistrationUrl);
    }

    public Task SendWebhook(EmbedBuilder builder, WebhookDestination dest = WebhookDestination.Public)
        => this.SendWebhook(builder.Build(), dest);

    public async Task SendWebhook(Embed embed, WebhookDestination dest = WebhookDestination.Public)
    {
        if (this.discordConfiguration.DiscordIntegrationEnabled) return;
        
        DiscordWebhookClient client = dest switch
        {
            WebhookDestination.Public => this.publicClient,
            WebhookDestination.Moderation => this.moderationClient,
            WebhookDestination.Registration => this.registrationClient,
            _ => throw new ArgumentOutOfRangeException(nameof(dest), dest, null),
        };
        if (client == null) return;

        await client.SendMessageAsync
        (
            embeds: new[]
            {
                embed,
            }
        );
    }

    public DiscordColor GetEmbedColor()
    {
        Color embedColor = Color.ParseHex(this.discordConfiguration.EmbedColor);
        Rgb24 pixel = embedColor.ToPixel<Rgb24>();
        return new DiscordColor(pixel.R, pixel.G, pixel.B);
    }

    public Task SendWebhook(string title, string description, WebhookDestination dest = WebhookDestination.Public)
        => this.SendWebhook
        (
            new EmbedBuilder
            {
                Title = title,
                Description = description,
                Color = this.GetEmbedColor(),
            },
            dest
        );
}