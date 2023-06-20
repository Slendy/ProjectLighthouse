using System.Collections.Generic;
using Redis.OM.Modeling;

namespace LBPUnion.ProjectLighthouse.Types.Users;

[Document(StorageType = StorageType.Json, Prefixes = new[] {"UserFriendData",})]
public class UserFriendData
{
    [RedisIdField]
    public int UserId { get; set; }

    [Indexed]
    public List<int> FriendIds { get; set; }

    [Indexed]
    public List<int> BlockedIds { get; set; }
}