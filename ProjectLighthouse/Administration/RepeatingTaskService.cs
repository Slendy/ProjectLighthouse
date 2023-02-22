using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using LBPUnion.ProjectLighthouse.Administration.Maintenance;
using LBPUnion.ProjectLighthouse.Database;
using LBPUnion.ProjectLighthouse.Logging;
using LBPUnion.ProjectLighthouse.Types.Logging;
using LBPUnion.ProjectLighthouse.Types.Maintenance;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LBPUnion.ProjectLighthouse.Administration;

public class RepeatingTaskService : BackgroundService
{
    private readonly IServiceScopeFactory scopeFactory;

    public RepeatingTaskService(IServiceScopeFactory scopeFactory)
    {
        this.scopeFactory = scopeFactory;
        Logger.Info("Initializing repeating tasks...", LogArea.Startup);
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        // Workaround for the method to be executed asynchronously, allowing startup to continue.
        await Task.Yield();
        Queue<IRepeatingTask> taskQueue = new();
        foreach (IRepeatingTask task in MaintenanceHelper.RepeatingTasks) taskQueue.Enqueue(task);
        while (!cancellationToken.IsCancellationRequested)
        {
            using IServiceScope scope = this.scopeFactory.CreateScope();
            await using DatabaseContext database = scope.ServiceProvider.GetRequiredService<DatabaseContext>();
            try
            {
                if (!taskQueue.TryDequeue(out IRepeatingTask task))
                {
                    Thread.Sleep(100);
                    continue;
                }

                Debug.Assert(task != null);

                if (task.LastRan + task.RepeatInterval <= DateTime.Now)
                {
                    await task.Run(database);
                    task.LastRan = DateTime.Now;

                    Logger.Debug($"Ran task \"{task.Name}\"", LogArea.Maintenance);
                }

                taskQueue.Enqueue(task);
                Thread.Sleep(500); // Doesn't need to be that precise.
            }
            catch (Exception e)
            {
                Logger.Warn($"Error occured while processing repeating tasks: \n{e}", LogArea.Maintenance);
            }
        }

        await Task.CompletedTask;
    }

}