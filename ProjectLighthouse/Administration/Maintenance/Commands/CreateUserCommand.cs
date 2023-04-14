#nullable enable
using System;
using System.Threading.Tasks;
using LBPUnion.ProjectLighthouse.Configuration;
using LBPUnion.ProjectLighthouse.Database;
using LBPUnion.ProjectLighthouse.Helpers;
using LBPUnion.ProjectLighthouse.Logging;
using LBPUnion.ProjectLighthouse.Types.Entities.Profile;
using LBPUnion.ProjectLighthouse.Types.Logging;
using LBPUnion.ProjectLighthouse.Types.Maintenance;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LBPUnion.ProjectLighthouse.Administration.Maintenance.Commands;

public class CreateUserCommand : ICommand
{
    public async Task Run(IServiceProvider serviceProvider, string[] args, Logger logger)
    {
        string onlineId = args[0];
        string password = args[1];

        password = CryptoHelper.Sha256Hash(password);

        await using DatabaseContext database = serviceProvider.GetRequiredService<DatabaseContext>();

        UserEntity? user = await database.Users.FirstOrDefaultAsync(u => u.Username == onlineId);
        if (user == null)
        {
            ServerConfiguration serverConfiguration = serviceProvider.GetRequiredService<ServerConfiguration>();
            user = await database.CreateUser(serverConfiguration, onlineId, CryptoHelper.BCryptHash(password));
            logger.LogSuccess($"Created user {user.UserId} with online ID (username) {user.Username} and the specified password.", LogArea.Command);

            user.PasswordResetRequired = true;
            logger.LogInfo("This user will need to reset their password when they log in.", LogArea.Command);

            await database.SaveChangesAsync();
            logger.LogInfo("Database changes saved.", LogArea.Command);
        }
        else
        {
            logger.LogError("A user with this username already exists.", LogArea.Command);
        }
    }

    public string Name() => "Create New User";

    public string[] Aliases()
        => new[]
        {
            "useradd", "adduser", "newuser", "createUser",
        };

    public string Arguments() => "<OnlineID> <Password>";

    public int RequiredArgs() => 2;
}