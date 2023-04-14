#nullable enable
using System.Security.Cryptography;
using LBPUnion.ProjectLighthouse.Configuration;
using LBPUnion.ProjectLighthouse.Database;
using LBPUnion.ProjectLighthouse.Extensions;
using LBPUnion.ProjectLighthouse.Helpers;
using LBPUnion.ProjectLighthouse.Types.Entities.Level;
using LBPUnion.ProjectLighthouse.Types.Entities.Token;
using LBPUnion.ProjectLighthouse.Types.Levels;
using LBPUnion.ProjectLighthouse.Types.Matchmaking.Rooms;
using LBPUnion.ProjectLighthouse.Types.Serialization;
using LBPUnion.ProjectLighthouse.Types.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LBPUnion.ProjectLighthouse.Servers.GameServer.Controllers.Slots;

[ApiController]
[Authorize]
[Route("LITTLEBIGPLANETPS3_XML/")]
[Produces("text/xml")]
public class SlotsController : ControllerBase
{
    private readonly DatabaseContext database;
    private readonly ServerConfiguration serverConfiguration;
    public SlotsController(DatabaseContext database, ServerConfiguration serverConfiguration)
    {
        this.database = database;
        this.serverConfiguration = serverConfiguration;
    }

    [HttpGet("slots/by")]
    public async Task<IActionResult> SlotsBy([FromQuery(Name = "u")] string username, [FromQuery] int pageStart, [FromQuery] int pageSize)
    {
        GameTokenEntity token = this.GetToken();

        if (pageSize <= 0) return this.BadRequest();

        int targetUserId = await this.database.UserIdFromUsername(username);
        if (targetUserId == 0) return this.NotFound();

        int usedSlots = this.database.Slots.Count(s => s.CreatorId == targetUserId);

        List<SlotBase> slots = (await this.database.Slots.Where(s => s.CreatorId == targetUserId)
            .ByGameVersion(token.GameVersion, token.UserId == targetUserId)
            .Skip(Math.Max(0, pageStart - 1))
            .Take(Math.Min(pageSize, usedSlots))
            .ToListAsync()).ToSerializableList(s => SlotBase.CreateFromEntity(s, token));
        
        int start = pageStart + Math.Min(pageSize, usedSlots);
        int total = await this.database.Slots.CountAsync(s => s.CreatorId == targetUserId);

        return this.Ok(new GenericSlotResponse("slots", slots, total, start));
    }

    [HttpGet("slotList")]
    public async Task<IActionResult> GetSlotListAlt([FromQuery(Name = "s")] int[] slotIds)
    {
        GameTokenEntity token = this.GetToken();

        List<SlotBase> slots = new();
        foreach (int slotId in slotIds)
        {
            SlotEntity? slot = await this.database.Slots.Include(t => t.Creator).Where(t => t.SlotId == slotId && t.Type == SlotType.User).FirstOrDefaultAsync();
            if (slot == null)
            {
                slot = await this.database.Slots.Where(t => t.InternalSlotId == slotId && t.Type == SlotType.Developer).FirstOrDefaultAsync();
                if (slot == null)
                {
                    slots.Add(new GameDeveloperSlot
                    {
                        SlotId = slotId,
                    });
                    continue;
                }
            }
            
            slots.Add(SlotBase.CreateFromEntity(slot, token));
        }

        return this.Ok(new GenericSlotResponse(slots, slots.Count, 0));
    }

    [HttpGet("slots/developer")]
    public async Task<IActionResult> StoryPlayers()
    {
        GameTokenEntity token = this.GetToken();

        List<int> activeSlotIds = RoomHelper.Rooms.Where(r => r.Slot.SlotType == SlotType.Developer).Select(r => r.Slot.SlotId).ToList();

        List<SlotBase> slots = new();

        foreach (int id in activeSlotIds)
        {
            int placeholderSlotId = await SlotHelper.GetPlaceholderSlotId(this.database, id, SlotType.Developer);
            SlotEntity slot = await this.database.Slots.FirstAsync(s => s.SlotId == placeholderSlotId);

            slots.Add(SlotBase.CreateFromEntity(slot, token));
        }

        return this.Ok(new GenericSlotResponse(slots));
    }

    [HttpGet("s/developer/{id:int}")]
    public async Task<IActionResult> SDev(int id)
    {
        GameTokenEntity token = this.GetToken();

        int slotId = await SlotHelper.GetPlaceholderSlotId(this.database, id, SlotType.Developer);
        SlotEntity slot = await this.database.Slots.FirstAsync(s => s.SlotId == slotId);

        return this.Ok(SlotBase.CreateFromEntity(slot, token));
    } 

    [HttpGet("s/user/{id:int}")]
    public async Task<IActionResult> SUser(int id)
    {
        GameTokenEntity token = this.GetToken();

        GameVersion gameVersion = token.GameVersion;

        SlotEntity? slot = await this.database.Slots.ByGameVersion(gameVersion, true, true).FirstOrDefaultAsync(s => s.SlotId == id);

        if (slot == null) return this.NotFound();

        return this.Ok(SlotBase.CreateFromEntity(slot, token, SerializationMode.Full));
    }

    [HttpGet("slots/cool")]
    public async Task<IActionResult> Lbp1CoolSlots([FromQuery] int page)
    {
        const int pageSize = 30;
        return await this.CoolSlots((page - 1) * pageSize, pageSize);
    }

    [HttpGet("slots/lbp2cool")]
    public async Task<IActionResult> CoolSlots
    (
        [FromQuery] int pageStart,
        [FromQuery] int pageSize,
        [FromQuery] string? gameFilterType = null,
        [FromQuery] int? players = null,
        [FromQuery] bool? move = null,
        [FromQuery] int? page = null
    )
    {
        if (page != null) pageStart = (int)page * 30;
        // bit of a better placeholder until we can track average user interaction with /stream endpoint
        return await this.ThumbsSlots(pageStart, Math.Min(pageSize, 30), gameFilterType, players, move, "thisWeek");
    }

    [HttpGet("slots")]
    public async Task<IActionResult> NewestSlots([FromQuery] int pageStart, [FromQuery] int pageSize)
    {
        GameTokenEntity token = this.GetToken();

        if (pageSize <= 0) return this.BadRequest();

        GameVersion gameVersion = token.GameVersion;

        List<SlotBase> slots = (await this.database.Slots.ByGameVersion(gameVersion, false, true)
            .OrderByDescending(s => s.FirstUploaded)
            .Skip(Math.Max(0, pageStart - 1))
            .Take(Math.Min(pageSize, 30))
            .ToListAsync()).ToSerializableList(s => SlotBase.CreateFromEntity(s, token));

        int start = pageStart + Math.Min(pageSize, this.serverConfiguration.UserGeneratedContentLimits.EntitledSlots);
        int total = await StatisticsHelper.SlotCountForGame(this.database, token.GameVersion);
        return this.Ok(new GenericSlotResponse(slots, total, start));
    }

    [HttpGet("slots/like/{slotType}/{slotId:int}")]
    public async Task<IActionResult> SimilarSlots([FromRoute] string slotType, [FromRoute] int slotId, [FromQuery] int pageStart, [FromQuery] int pageSize)
    {
        GameTokenEntity token = this.GetToken();

        if (pageSize <= 0) return this.BadRequest();

        if (slotType != "user") return this.BadRequest();

        GameVersion gameVersion = token.GameVersion;

        SlotEntity? targetSlot = await this.database.Slots.FirstOrDefaultAsync(s => s.SlotId == slotId);
        if (targetSlot == null) return this.BadRequest();

        string[] tags = targetSlot.LevelTags(this.database);

        List<int> slotIdsWithTag = this.database.RatedLevels
            .Where(r => r.TagLBP1.Length > 0)
            .Where(r => tags.Contains(r.TagLBP1))
            .Select(r => r.SlotId)
            .ToList();

        List<SlotBase> slots = (await this.database.Slots.ByGameVersion(gameVersion, false, true)
            .Where(s => slotIdsWithTag.Contains(s.SlotId))
            .OrderByDescending(s => s.PlaysLBP1)
            .Skip(Math.Max(0, pageStart - 1))
            .Take(Math.Min(pageSize, 30))
            .ToListAsync()).ToSerializableList(s => SlotBase.CreateFromEntity(s, token));

        int start = pageStart + Math.Min(pageSize, this.serverConfiguration.UserGeneratedContentLimits.EntitledSlots);
        int total = slotIdsWithTag.Count;

        return this.Ok(new GenericSlotResponse(slots, total, start));
    }

    [HttpGet("slots/highestRated")]
    public async Task<IActionResult> HighestRatedSlots([FromQuery] int pageStart, [FromQuery] int pageSize)
    {
        GameTokenEntity token = this.GetToken();

        if (pageSize <= 0) return this.BadRequest();

        GameVersion gameVersion = token.GameVersion;

        List<SlotBase> slots = (await this.database.Slots.ByGameVersion(gameVersion, false, true)
            .ToAsyncEnumerable()
            .OrderByDescending(s => s.RatingLBP1(this.database))
            .Skip(Math.Max(0, pageStart - 1))
            .Take(Math.Min(pageSize, 30))
            .ToListAsync()).ToSerializableList(s => SlotBase.CreateFromEntity(s, token));

        int start = pageStart + Math.Min(pageSize, this.serverConfiguration.UserGeneratedContentLimits.EntitledSlots);
        int total = await StatisticsHelper.SlotCount(this.database); 

        return this.Ok(new GenericSlotResponse(slots, total, start));
    }

    [HttpGet("slots/tag")]
    public async Task<IActionResult> SimilarSlots([FromQuery] string tag, [FromQuery] int pageStart, [FromQuery] int pageSize)
    {
        GameTokenEntity token = this.GetToken();

        if (pageSize <= 0) return this.BadRequest();

        List<int> slotIdsWithTag = await this.database.RatedLevels.Where(r => r.TagLBP1.Length > 0)
            .Where(r => r.TagLBP1 == tag)
            .Select(s => s.SlotId)
            .ToListAsync();

        List<SlotBase> slots = (await this.database.Slots.Where(s => slotIdsWithTag.Contains(s.SlotId))
            .ByGameVersion(token.GameVersion, false, true)
            .OrderByDescending(s => s.PlaysLBP1)
            .Skip(Math.Max(0, pageStart - 1))
            .Take(Math.Min(pageSize, 30))
            .ToListAsync()).ToSerializableList(s => SlotBase.CreateFromEntity(s, token));

        int start = pageStart + Math.Min(pageSize, this.serverConfiguration.UserGeneratedContentLimits.EntitledSlots);
        int total = slotIdsWithTag.Count;

        return this.Ok(new GenericSlotResponse(slots, total, start));
    }

    [HttpGet("slots/mmpicks")]
    public async Task<IActionResult> TeamPickedSlots([FromQuery] int pageStart, [FromQuery] int pageSize)
    {
        GameTokenEntity token = this.GetToken();

        if (pageSize <= 0) return this.BadRequest();

        List<SlotBase> slots = (await this.database.Slots.Where(s => s.TeamPick)
            .ByGameVersion(token.GameVersion, false, true)
            .OrderByDescending(s => s.LastUpdated)
            .Skip(Math.Max(0, pageStart - 1))
            .Take(Math.Min(pageSize, 30))
            .ToListAsync()).ToSerializableList(s => SlotBase.CreateFromEntity(s, token));
        int start = pageStart + Math.Min(pageSize, this.serverConfiguration.UserGeneratedContentLimits.EntitledSlots);
        int total = await StatisticsHelper.TeamPickCountForGame(this.database, token.GameVersion);

        return this.Ok(new GenericSlotResponse(slots, total, start));
    }

    [HttpGet("slots/lbp2luckydip")]
    public async Task<IActionResult> LuckyDipSlots([FromQuery] int pageStart, [FromQuery] int pageSize, [FromQuery] int seed)
    {
        GameTokenEntity token = this.GetToken();

        if (pageSize <= 0) return this.BadRequest();

        GameVersion gameVersion = token.GameVersion;

        List<SlotBase> slots = (await this.database.Slots.ByGameVersion(gameVersion, false, true)
            .OrderBy(_ => EF.Functions.Random())
            .Take(Math.Min(pageSize, 30))
            .ToListAsync()).ToSerializableList(s => SlotBase.CreateFromEntity(s, token));

        int start = pageStart + Math.Min(pageSize, this.serverConfiguration.UserGeneratedContentLimits.EntitledSlots);
        int total = await StatisticsHelper.SlotCountForGame(this.database, token.GameVersion);

        return this.Ok(new GenericSlotResponse(slots, total, start));
    }

    [HttpGet("slots/thumbs")]
    public async Task<IActionResult> ThumbsSlots
    (
        [FromQuery] int pageStart,
        [FromQuery] int pageSize,
        [FromQuery] string? gameFilterType = null,
        [FromQuery] int? players = null,
        [FromQuery] bool? move = null,
        [FromQuery] string? dateFilterType = null
    )
    {
        GameTokenEntity token = this.GetToken();

        if (pageSize <= 0) return this.BadRequest();

        Random rand = new();

        List<SlotBase> slots = (await this.filterByRequest(gameFilterType, dateFilterType, token.GameVersion)
            .AsAsyncEnumerable()
            .OrderByDescending(s => s.Thumbsup(this.database))
            .ThenBy(_ => rand.Next())
            .Skip(Math.Max(0, pageStart - 1))
            .Take(Math.Min(pageSize, 30))
            .ToListAsync()).ToSerializableList(s => SlotBase.CreateFromEntity(s, token));

        int start = pageStart + Math.Min(pageSize, this.serverConfiguration.UserGeneratedContentLimits.EntitledSlots);
        int total = await StatisticsHelper.SlotCountForGame(this.database, token.GameVersion);

        return this.Ok(new GenericSlotResponse(slots, total, start));
    }

    [HttpGet("slots/mostUniquePlays")]
    public async Task<IActionResult> MostUniquePlaysSlots
    (
        [FromQuery] int pageStart,
        [FromQuery] int pageSize,
        [FromQuery] string? gameFilterType = null,
        [FromQuery] int? players = null,
        [FromQuery] bool? move = null,
        [FromQuery] string? dateFilterType = null
    )
    {
        GameTokenEntity token = this.GetToken();

        if (pageSize <= 0) return this.BadRequest();

        Random rand = new();

        List<SlotBase> slots = (await this.filterByRequest(gameFilterType, dateFilterType, token.GameVersion)
            .AsAsyncEnumerable()
            .OrderByDescending(
                // probably not the best way to do this?
                s =>
                {
                    return getGameFilter(gameFilterType, token.GameVersion) switch
                    {
                        GameVersion.LittleBigPlanet1 => s.PlaysLBP1Unique,
                        GameVersion.LittleBigPlanet2 => s.PlaysLBP2Unique,
                        GameVersion.LittleBigPlanet3 => s.PlaysLBP3Unique,
                        GameVersion.LittleBigPlanetVita => s.PlaysLBP2Unique,
                        _ => s.PlaysUnique,
                    };
                })
            .ThenBy(_ => rand.Next())
            .Skip(Math.Max(0, pageStart - 1))
            .Take(Math.Min(pageSize, 30))
            .ToListAsync()).ToSerializableList(s => SlotBase.CreateFromEntity(s, token));

        int start = pageStart + Math.Min(pageSize, this.serverConfiguration.UserGeneratedContentLimits.EntitledSlots);
        int total = await StatisticsHelper.SlotCountForGame(this.database, token.GameVersion);

        return this.Ok(new GenericSlotResponse(slots, total, start));
    }

    [HttpGet("slots/mostHearted")]
    public async Task<IActionResult> MostHeartedSlots
    (
        [FromQuery] int pageStart,
        [FromQuery] int pageSize,
        [FromQuery] string? gameFilterType = null,
        [FromQuery] int? players = null,
        [FromQuery] bool? move = null,
        [FromQuery] string? dateFilterType = null
    )
    {
        GameTokenEntity token = this.GetToken();

        if (pageSize <= 0) return this.BadRequest();

        List<SlotBase> slots = (await this.filterByRequest(gameFilterType, dateFilterType, token.GameVersion)
            .AsAsyncEnumerable()
            .OrderByDescending(s => s.Hearts(this.database))
            .ThenBy(_ => RandomNumberGenerator.GetInt32(int.MaxValue))
            .Skip(Math.Max(0, pageStart - 1))
            .Take(Math.Min(pageSize, 30))
            .ToListAsync()).ToSerializableList(s => SlotBase.CreateFromEntity(s, token));

        int start = pageStart + Math.Min(pageSize, this.serverConfiguration.UserGeneratedContentLimits.EntitledSlots);
        int total = await StatisticsHelper.SlotCountForGame(this.database, token.GameVersion);

        return this.Ok(new GenericSlotResponse(slots, total, start));
    }
    
    // /slots/busiest?pageStart=1&pageSize=30&gameFilterType=both&players=1&move=true
    [HttpGet("slots/busiest")]
    public async Task<IActionResult> BusiestLevels
    (
        [FromQuery] int pageStart,
        [FromQuery] int pageSize,
        [FromQuery] string? gameFilterType = null,
        [FromQuery] int? players = null,
        [FromQuery] bool? move = null
    )
    {
        GameTokenEntity token = this.GetToken();

        if (pageSize <= 0) return this.BadRequest();

        Dictionary<int, int> playersBySlotId = new();

        foreach (Room room in RoomHelper.Rooms)
        {
            // TODO: support developer slotTypes?
            if (room.Slot.SlotType != SlotType.User) continue;

            if (!playersBySlotId.TryGetValue(room.Slot.SlotId, out int playerCount)) 
                playersBySlotId.Add(room.Slot.SlotId, 0);

            playerCount += room.PlayerIds.Count;

            playersBySlotId.Remove(room.Slot.SlotId);
            playersBySlotId.Add(room.Slot.SlotId, playerCount);
        }

        IEnumerable<int> orderedPlayersBySlotId = playersBySlotId
            .Skip(Math.Max(0, pageStart - 1))
            .Take(Math.Min(pageSize, 30))
            .OrderByDescending(kvp => kvp.Value)
            .Select(kvp => kvp.Key);
        
        List<SlotBase> slots = new();

        foreach (int slotId in orderedPlayersBySlotId)
        {
            SlotEntity? slot = await this.database.Slots.ByGameVersion(token.GameVersion, false, true)
                .Where(s => s.SlotId == slotId)
                .FirstOrDefaultAsync();
            if (slot == null) continue; // shouldn't happen ever unless the room is borked
            
            slots.Add(SlotBase.CreateFromEntity(slot, token));
        }

        int start = pageStart + Math.Min(pageSize, this.serverConfiguration.UserGeneratedContentLimits.EntitledSlots);
        int total = playersBySlotId.Count;

        return this.Ok(new GenericSlotResponse(slots, total, start));
    }


    private static GameVersion getGameFilter(string? gameFilterType, GameVersion version)
    {
        return version switch
        {
            GameVersion.LittleBigPlanetVita => GameVersion.LittleBigPlanetVita,
            GameVersion.LittleBigPlanetPSP => GameVersion.LittleBigPlanetPSP,
            _ => gameFilterType switch
            {
                "lbp1" => GameVersion.LittleBigPlanet1,
                "lbp2" => GameVersion.LittleBigPlanet2,
                "lbp3" => GameVersion.LittleBigPlanet3,
                "both" => GameVersion.LittleBigPlanet2, // LBP2 default option
                null => GameVersion.LittleBigPlanet1,
                _ => GameVersion.Unknown,
            },
        };
    }

    private IQueryable<SlotEntity> filterByRequest(string? gameFilterType, string? dateFilterType, GameVersion version)
    {
        if (version == GameVersion.LittleBigPlanetVita || version == GameVersion.LittleBigPlanetPSP || version == GameVersion.Unknown)
        {
            return this.database.Slots.ByGameVersion(version, false, true);
        }

        string _dateFilterType = dateFilterType ?? "";

        long oldestTime = _dateFilterType switch
        {
            "thisWeek" => DateTimeOffset.Now.AddDays(-7).ToUnixTimeMilliseconds(),
            "thisMonth" => DateTimeOffset.Now.AddDays(-31).ToUnixTimeMilliseconds(),
            _ => 0,
        };

        GameVersion gameVersion = getGameFilter(gameFilterType, version);

        IQueryable<SlotEntity> whereSlots;

        // ReSharper disable once ConvertIfStatementToConditionalTernaryExpression
        if (gameFilterType == "both")
            // Get game versions less than the current version
            // Needs support for LBP3 ("both" = LBP1+2)
            whereSlots = this.database.Slots.Where(s => s.Type == SlotType.User && !s.Hidden && s.GameVersion <= gameVersion && s.FirstUploaded >= oldestTime);
        else
            // Get game versions exactly equal to gamefiltertype
            whereSlots = this.database.Slots.Where(s => s.Type == SlotType.User && !s.Hidden && s.GameVersion == gameVersion && s.FirstUploaded >= oldestTime);

        return whereSlots.Include(s => s.Creator);
    }
}