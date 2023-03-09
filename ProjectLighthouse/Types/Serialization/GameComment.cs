﻿using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;
using LBPUnion.ProjectLighthouse.Database;
using LBPUnion.ProjectLighthouse.Serialization;
using LBPUnion.ProjectLighthouse.Types.Entities.Profile;
using Microsoft.EntityFrameworkCore;

namespace LBPUnion.ProjectLighthouse.Types.Serialization;

[XmlRoot("comment")]
[XmlType("comment")]
public class GameComment : ILbpSerializable, INeedsPreparationForSerialization
{
    [XmlIgnore]
    public int TargetUserId { get; set; }

    [XmlElement("id")]
    public int CommentId { get; set; }

    [XmlElement("npHandle")]
    public string AuthorUsername { get; set; }

    [XmlElement("timestamp")]
    public long Timestamp { get; set; }

    [DefaultValue(false)]
    [XmlElement("deleted")]
    public bool Deleted { get; set; }

    [DefaultValue("")]
    [XmlElement("deletedBy")]
    public string DeletedBy { get; set; }

    [DefaultValue("")]
    [XmlElement("deletedType")]
    public string DeletedType { get; set; }

    [XmlElement("message")]
    public string Message { get; set; }

    [XmlElement("thumbsup")]
    public int ThumbsUp { get; set; }

    [XmlElement("thumbsdown")]
    public int ThumbsDown { get; set; }

    [XmlElement("yourthumb")]
    public int YourThumb { get; set; }

    public async Task PrepareForSerialization(DatabaseContext database)
    {
        this.AuthorUsername = await database.Users.Where(u => u.UserId == this.TargetUserId)
            .Select(u => u.Username)
            .FirstOrDefaultAsync();

        this.YourThumb = await database.Reactions.Where(r => r.UserId == this.TargetUserId)
            .Where(r => r.TargetId == this.CommentId)
            .Select(r => r.Rating)
            .FirstOrDefaultAsync();
    }

    public static GameComment CreateFromEntity(CommentEntity comment) =>
        new()
        {
            CommentId = comment.CommentId,
            Message = comment.Message,
            Timestamp = comment.Timestamp,
            ThumbsUp = comment.ThumbsUp,
            ThumbsDown = comment.ThumbsDown,
            Deleted = comment.Deleted,
            DeletedBy = comment.DeletedBy,
            DeletedType = comment.DeletedType,
        };

    public static CommentEntity ConvertToEntity(GameComment comment) => new()
    {
        Message = comment.Message,
    };

}