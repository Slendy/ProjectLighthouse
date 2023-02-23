#nullable enable
using System.Linq;
using System.Threading.Tasks;
using LBPUnion.ProjectLighthouse.Database;
using LBPUnion.ProjectLighthouse.Types.Entities.Profile;
using LBPUnion.ProjectLighthouse.Types.Entities.Token;
using LBPUnion.ProjectLighthouse.Types.Users;
using Microsoft.EntityFrameworkCore;

namespace LBPUnion.ProjectLighthouse.Helpers;

public static class LastContactHelper
{

    public static async Task SetLastContact(DatabaseContext database, GameToken token, GameVersion gameVersion, Platform platform)
    {
        LastContact? lastContact = await database.LastContacts.Where(l => l.UserId == token.UserId).FirstOrDefaultAsync();

        if (lastContact == null)
        {
            lastContact = new LastContact
            {
                UserId = token.UserId,
            };
            database.LastContacts.Add(lastContact);
        }

        lastContact.Timestamp = TimeHelper.Timestamp;
        lastContact.GameVersion = gameVersion;
        lastContact.Platform = platform;

        await database.SaveChangesAsync();
    }
}