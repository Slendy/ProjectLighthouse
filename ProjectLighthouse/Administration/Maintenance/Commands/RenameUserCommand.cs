#nullable enable
using System;
using System.Threading.Tasks;
using LBPUnion.ProjectLighthouse.Database;
using LBPUnion.ProjectLighthouse.Logging;
using LBPUnion.ProjectLighthouse.Types.Entities.Profile;
using LBPUnion.ProjectLighthouse.Types.Logging;
using LBPUnion.ProjectLighthouse.Types.Maintenance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LBPUnion.ProjectLighthouse.Administration.Maintenance.Commands;

public class RenameUserCommand : ICommand
{
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
                logger.LogError($"Could not find user by parameter '{args[0]}'", LogArea.Command);
                return;
            }

        user.Username = args[1];
        await database.SaveChangesAsync();

        logger.LogSuccess($"The username for user {user.Username} (id: {user.UserId}) has been changed.", LogArea.Command);
    }
    public string Name() => "Rename User";
    public string[] Aliases()
        => new[]
        {
            "renameUser",
        };
    public string Arguments() => "<username/userId> <newUsername>";
    public int RequiredArgs() => 2;
}