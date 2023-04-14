using LBPUnion.ProjectLighthouse.Configuration;
using LBPUnion.ProjectLighthouse.Logging.Loggers.AspNet;
using LBPUnion.ProjectLighthouse.Servers.GameServer.Startup;
using LBPUnion.ProjectLighthouse.Types.Misc;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LBPUnion.ProjectLighthouse.Servers.GameServer;

public static class Program
{
    public static void Main(string[] args)
    {
        StartupTasks.Run(args, ServerType.GameServer);

        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(ServerConfiguration serverConfiguration, string[] args)
        => Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults
            (
                webBuilder =>
                {
                    webBuilder.UseStartup<GameServerStartup>();
                    webBuilder.UseUrls(serverConfiguration.GameApiListenUrl);
                }
            )
            .ConfigureLogging
            (
                logging =>
                {
                    logging.ClearProviders();
                    logging.Services.TryAddEnumerable(ServiceDescriptor.Singleton<ILoggerProvider, AspNetToLighthouseLoggerProvider>());
                }
            );
}