using System;
using System.ComponentModel.DataAnnotations;

namespace LBPUnion.ProjectLighthouse.Types.Entities.Token;

public class WebTokenEntity
{
    // ReSharper disable once UnusedMember.Global
    [Key]
    public int TokenId { get; set; }

    public int UserId { get; set; }

    public string UserToken { get; set; }

    public DateTimeOffset ExpiresAt { get; set; }

    public bool Verified { get; set; }
}