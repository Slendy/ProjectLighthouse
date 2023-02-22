using System.ComponentModel.DataAnnotations;
using Redis.OM.Modeling;

namespace LBPUnion.ProjectLighthouse.Types.Redis;

[Document(StorageType = StorageType.Json, Prefixes = new[] {"User",})]
public class RedisUser
{
    [Key]
    public int UserId { get; set; }

    /// <summary>
    /// Used as a quick lookup for username
    /// </summary>
    [Indexed]
    public string Username { get; set; }

    /// <summary>
    /// Used to track user sessions
    /// </summary>
    [Indexed]
    public int[] GameTokens { get; set; }

    /// <summary>
    /// A user can be signed in on multiple platforms at the same time
    /// i.e. they can be in multiple rooms at the same time.
    /// </summary>
    [Indexed]
    public int[] Rooms { get; set; }

    /// <summary>
    /// A list of the users friends, updated when the user first logs in
    /// </summary>
    public string[] FriendList { get; set; }

    /// <summary>
    /// A list of the users blocked players, updated when the user first logs in
    /// </summary>
    public string[] BlockedList { get; set; }

}