using System;
using System.Threading.Tasks;
using LBPUnion.ProjectLighthouse.Database;
using LBPUnion.ProjectLighthouse.Types.Maintenance;

namespace LBPUnion.ProjectLighthouse.Administration.Maintenance.MaintenanceJobs;

public class DeleteAllTokensMaintenanceJob : IMaintenanceJob
{
    public string Name() => "Delete ALL Tokens";
    public string Description() => "Deletes ALL game tokens and web tokens.";
    public async Task Run(DatabaseContext database)
    {
        database.GameTokens.RemoveRange(database.GameTokens);
        database.WebTokens.RemoveRange(database.WebTokens);

        await database.SaveChangesAsync();

        Console.WriteLine("Deleted ALL tokens.");
    }
}