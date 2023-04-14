using LBPUnion.ProjectLighthouse.Configuration;
using LBPUnion.ProjectLighthouse.Logging.Loggers.AspNet;
using LBPUnion.ProjectLighthouse.Servers.API.Startup;
using LBPUnion.ProjectLighthouse.Types.Misc;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LBPUnion.ProjectLighthouse.Servers.API;

public static class Program
{
    public static void Main(string[] args)
    {
        StartupTasks.Run(args, ServerType.Api);

        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(ServerConfiguration serverConfiguration, string[] args)
        => Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults
            (
                webBuilder =>
                {
                    
                    webBuilder.UseStartup<ApiStartup>();
                    webBuilder.UseUrls(serverConfiguration.ApiListenUrl);
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