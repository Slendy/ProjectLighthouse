using System.Linq;
using System.Threading.Tasks;
using LBPUnion.ProjectLighthouse.Database;
using LBPUnion.ProjectLighthouse.Extensions;
using LBPUnion.ProjectLighthouse.Types.Levels;
using LBPUnion.ProjectLighthouse.Filter;
using LBPUnion.ProjectLighthouse.Types.Users;
using Microsoft.EntityFrameworkCore;

namespace LBPUnion.ProjectLighthouse.Helpers;

public static class StatisticsHelper
{

    public static async Task<int> RecentMatches(DatabaseContext database) => await database.LastContacts.CountAsync(l => TimeHelper.Timestamp - l.Timestamp < 300);

    public static async Task<int> RecentMatchesForGame(DatabaseContext database, GameVersion gameVersion)
        => await database.LastContacts.CountAsync(l => TimeHelper.Timestamp - l.Timestamp < 300 && l.GameVersion == gameVersion);

    public static async Task<int> SlotCount(DatabaseContext database) => await database.Slots.CountAsync(s => s.Type == SlotType.User);

    public static async Task<int> SlotCount(DatabaseContext database, SlotQueryBuilder queryBuilder) => await database.Slots.Where(queryBuilder.Build()).CountAsync();

    public static async Task<int> UserCount(DatabaseContext database) => await database.Users.RemoveHiddenUsers().CountAsync();

    public static int RoomCountForPlatform(Platform targetPlatform) => RoomHelper.Rooms.Count(r => r.IsLookingForPlayers && r.RoomPlatform == targetPlatform);

    public static async Task<int> PhotoCount(DatabaseContext database) => await database.Photos.CountAsync();
    
    #region Moderator/Admin specific
    public static async Task<int> ReportCount(DatabaseContext database) => await database.Reports.CountAsync();
    public static async Task<int> CaseCount(DatabaseContext database) => await database.Cases.CountAsync();
    public static async Task<int> DismissedCaseCount(DatabaseContext database) => await database.Cases.CountAsync(c => c.DismissedAt != null);
    #endregion

    public static async Task<int> ApiKeyCount(DatabaseContext database) => await database.APIKeys.CountAsync();
}