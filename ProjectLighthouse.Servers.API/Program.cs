using LBPUnion.ProjectLighthouse.Configuration;
using LBPUnion.ProjectLighthouse.Configuration.ConfigurationCategories;
using LBPUnion.ProjectLighthouse.Extensions;
using LBPUnion.ProjectLighthouse.Logging.Loggers.AspNet;
using LBPUnion.ProjectLighthouse.Servers.API.Startup;
using LBPUnion.ProjectLighthouse.Types.Misc;
using LBPUnion.ProjectLighthouse.Types.Users;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace LBPUnion.ProjectLighthouse.Servers.API;

public static class Program
{
    public static void Main(string[] args)
    {
        StartupTasks.Run(args, ServerType.Api);

        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args)
        => Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((context, builder) =>
            {
                builder.SetBasePath(context.HostingEnvironment.ContentRootPath);
                builder.AddJsonFile(null, "test.json", false, false);
                builder.AddYamlFile(new Configuration.V2.ServerConfiguration(), null, "bruh.yml", false, false);
                IConfigurationRoot config = builder.Build();
                Console.WriteLine("Built new config");
                Configuration.V2.ServerConfiguration? svc = config.Get<Configuration.V2.ServerConfiguration>();
                Console.WriteLine(svc?.DbConnectionString ?? "null");
            })
            .ConfigureWebHostDefaults
            (
                webBuilder =>
                {
                    webBuilder.UseStartup<ApiStartup>();
                    webBuilder.UseUrls(ServerConfiguration.Instance.ApiListenUrl);
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