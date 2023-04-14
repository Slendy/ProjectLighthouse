#nullable enable
using System;
using System.Linq;
using System.Threading.Tasks;
using LBPUnion.ProjectLighthouse.Database;
using LBPUnion.ProjectLighthouse.Logging;
using LBPUnion.ProjectLighthouse.Types.Entities.Profile;
using LBPUnion.ProjectLighthouse.Types.Maintenance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LBPUnion.ProjectLighthouse.Administration.Maintenance.Commands;

public class WipeTokensForUserCommand : ICommand
{
    public string Name() => "Wipe tokens for user";
    public string[] Aliases()
        => new[]
        {
            "wipeTokens", "wipeToken", "deleteTokens", "deleteToken", "removeTokens", "removeToken",
        };
    public string Arguments() => "<username/userId>";
    public int RequiredArgs() => 1;
    public async Task Run(IServiceProvider serviceProvider, string[] args, Logger logger)
    {
        await using DatabaseContext database = serviceProvider.GetRequiredService<DatabaseContext>();
        UserEntity? user = await database.Users.FirstOrDefaultAsync(u => u.Username == args[0]);
        if (user == null)
            try
            {
                user = await database.Users.FirstOrDefaultAsync(u => u.UserId == Convert.ToInt32(args[0]));
                if (user == null) throw new Exception();
            }
            catch
            {
                Console.WriteLine(@$"Could not find user by parameter '{args[0]}'");
                return;
            }

        database.GameTokens.RemoveRange(database.GameTokens.Where(t => t.UserId == user.UserId));
        database.WebTokens.RemoveRange(database.WebTokens.Where(t => t.UserId == user.UserId));

        await database.SaveChangesAsync();

        Console.WriteLine(@$"Deleted all tokens for {user.Username} (id: {user.UserId}).");
    }
}