#nullable enable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using LBPUnion.ProjectLighthouse.Database;
using LBPUnion.ProjectLighthouse.Types.Entities.Level;
using LBPUnion.ProjectLighthouse.Types.Entities.Profile;

namespace LBPUnion.ProjectLighthouse.Types.Entities.Cache;

public class SlotCache
{
    [Key, ForeignKey("Slot"),]
    public required int SlotId { get; set; }

    public SlotEntity? Slot { get; set; }

    public string CreatorUsername { get; set; } = "";

    public string CreatorIcon { get; set; } = "";

    public int HeartCount { get; set; }

    public int CommentCount { get; set; }

    public int ReviewCount { get; set; }

    public int PhotoCount { get; set; }

    public int AuthorPhotoCount { get; set; }

    public double RatingLBP1 { get; set; }

    public int ThumbsUp { get; set; }

    public int ThumbsDown { get; set; }

    public string LevelTags { get; set; } = "";

    public static SlotCache CreateFromEntity(DatabaseContext database, SlotEntity slot) =>
        new()
        {
            SlotId = slot.SlotId,
            CreatorUsername = database.Users.Where(u => u.UserId == slot.CreatorId).Select(u => u.Username).First(),
            CreatorIcon = database.Users.Where(u => u.UserId == slot.CreatorId).Select(u => u.IconHash).First(),
            HeartCount = database.HeartedLevels.Count(hl => hl.SlotId == slot.SlotId),
            CommentCount = database.Comments.Count(c => c.TargetId == slot.SlotId && c.Type == CommentType.Level),
            ReviewCount = database.Reviews.Count(r => r.SlotId == slot.SlotId),
            PhotoCount = database.Photos.Count(p => p.SlotId == slot.SlotId),
            AuthorPhotoCount = database.Photos.Count(p => p.SlotId == slot.SlotId && p.CreatorId == slot.CreatorId),
            RatingLBP1 = database.RatedLevels.Where(r => r.SlotId == slot.SlotId).Average(r => (double?)r.RatingLBP1) ?? 3.0,
            ThumbsUp = database.RatedLevels.Count(r => r.SlotId == slot.SlotId && r.Rating == 1),
            ThumbsDown = database.RatedLevels.Count(r => r.SlotId == slot.SlotId && r.Rating == -1),
            LevelTags = string.Join(',', slot.LevelTags(database)),
        };

}