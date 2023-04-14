#nullable enable
using LBPUnion.ProjectLighthouse.Configuration;
using LBPUnion.ProjectLighthouse.Database;
using LBPUnion.ProjectLighthouse.Helpers;
using LBPUnion.ProjectLighthouse.Types.Entities.Level;
using LBPUnion.ProjectLighthouse.Types.Entities.Profile;
using LBPUnion.ProjectLighthouse.Types.Webhook;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LBPUnion.ProjectLighthouse.Servers.Website.Controllers.Moderator;

[ApiController]
[Route("moderation/slot/{id:int}")]
public class ModerationSlotController : ControllerBase
{
    private readonly DatabaseContext database;
    private readonly WebhookService webhookService;
    private readonly ServerConfiguration serverConfiguration;

    public ModerationSlotController(DatabaseContext database, WebhookService webhookService, ServerConfiguration serverConfiguration)
    {
        this.database = database;
        this.webhookService = webhookService;
        this.serverConfiguration = serverConfiguration;
    }

    [HttpGet("teamPick")]
    public async Task<IActionResult> TeamPick([FromRoute] int id)
    {
        UserEntity? user = this.database.UserFromWebRequest(this.Request);
        if (user == null || !user.IsModerator) return this.StatusCode(403);

        SlotEntity? slot = await this.database.Slots.Include(s => s.Creator).FirstOrDefaultAsync(s => s.SlotId == id);
        if (slot == null) return this.NotFound();

        slot.TeamPick = true;

        // Send webhook with slot.Name and slot.Creator.Username
        await this.webhookService.SendWebhook("New Team Pick!", $"The level [**{slot.Name}**]({this.serverConfiguration.ExternalUrl}/slot/{slot.SlotId}) by **{slot.Creator?.Username}** has been team picked");

        await this.database.SaveChangesAsync();

        return this.Redirect("~/slot/" + id);
    }

    [HttpGet("removeTeamPick")]
    public async Task<IActionResult> RemoveTeamPick([FromRoute] int id)
    {
        UserEntity? user = this.database.UserFromWebRequest(this.Request);
        if (user == null || !user.IsModerator) return this.StatusCode(403);

        SlotEntity? slot = await this.database.Slots.FirstOrDefaultAsync(s => s.SlotId == id);
        if (slot == null) return this.NotFound();

        slot.TeamPick = false;

        await this.database.SaveChangesAsync();

        return this.Redirect("~/slot/" + id);
    }

    [HttpGet("delete")]
    public async Task<IActionResult> DeleteLevel([FromRoute] int id)
    {
        UserEntity? user = this.database.UserFromWebRequest(this.Request);
        if (user == null || !user.IsModerator) return this.StatusCode(403);

        SlotEntity? slot = await this.database.Slots.FirstOrDefaultAsync(s => s.SlotId == id);
        if (slot == null) return this.Ok();

        await this.database.RemoveSlot(slot);

        return this.Redirect("~/slots/0");
    }
}
