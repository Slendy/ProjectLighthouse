#nullable enable
using System.Text.Json;
using LBPUnion.ProjectLighthouse.Configuration;
using LBPUnion.ProjectLighthouse.Database;
using LBPUnion.ProjectLighthouse.Extensions;
using LBPUnion.ProjectLighthouse.Files;
using LBPUnion.ProjectLighthouse.Helpers;
using LBPUnion.ProjectLighthouse.Types.Entities.Moderation;
using LBPUnion.ProjectLighthouse.Types.Entities.Token;
using LBPUnion.ProjectLighthouse.Types.Moderation.Reports;
using LBPUnion.ProjectLighthouse.Types.Serialization;
using LBPUnion.ProjectLighthouse.Types.Webhook;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LBPUnion.ProjectLighthouse.Servers.GameServer.Controllers;

[ApiController]
[Authorize]
[Route("LITTLEBIGPLANETPS3_XML/")]
[Produces("text/xml")]
public class ReportController : ControllerBase
{
    private readonly DatabaseContext database;
    private readonly ServerConfiguration serverConfiguration;
    private readonly WebhookService webhookService;

    public ReportController(DatabaseContext database, ServerConfiguration serverConfiguration, WebhookService webhookService)
    {
        this.database = database;
        this.serverConfiguration = serverConfiguration;
        this.webhookService = webhookService;
    }

    [HttpPost("grief")]
    public async Task<IActionResult> Report()
    {
        GameTokenEntity token = this.GetToken();

        string username = await this.database.UsernameFromGameToken(token);

        GameGriefReport? report = await this.DeserializeBody<GameGriefReport>();
        if (report == null) return this.BadRequest();

        if (string.IsNullOrWhiteSpace(report.JpegHash)) return this.BadRequest();

        if (!FileHelper.ResourceExists(report.JpegHash)) return this.BadRequest();

        if (report.XmlPlayers.Length > 4) return this.BadRequest();

        if (report.XmlPlayers.Any(p => !this.database.IsUsernameValid(p.Name))) return this.BadRequest();

        GriefReportEntity reportEntity = GameGriefReport.ConvertToEntity(report);

        reportEntity.Bounds = JsonSerializer.Serialize(report.XmlBounds.Rect, typeof(Rectangle));
        reportEntity.Players = JsonSerializer.Serialize(report.XmlPlayers, typeof(ReportPlayer[]));
        reportEntity.Timestamp = TimeHelper.TimestampMillis;
        reportEntity.ReportingPlayerId = token.UserId;

        this.database.Reports.Add(reportEntity);
        await this.database.SaveChangesAsync();

        await this.webhookService.SendWebhook(
            title: "New grief report",
            description: $"Submitted by {username}\n" +
                         $"To view it, click [here]({this.serverConfiguration.ExternalUrl}/moderation/report/{reportEntity.ReportId}).",
            dest: WebhookDestination.Moderation
        );

        return this.Ok();
    }

}