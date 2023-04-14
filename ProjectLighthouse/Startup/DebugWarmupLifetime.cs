using System;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using LBPUnion.ProjectLighthouse.Configuration;
using LBPUnion.ProjectLighthouse.Extensions;
using LBPUnion.ProjectLighthouse.Logging;
using LBPUnion.ProjectLighthouse.Types.Logging;
using LBPUnion.ProjectLighthouse.Types.Misc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace LBPUnion.ProjectLighthouse.Startup;

public class DebugWarmupLifetime : IHostLifetime
{
    public IHostApplicationLifetime ApplicationLifetime { get; }

    public ServerConfiguration ServerConfiguration { get; init; }

    private CancellationTokenRegistration applicationStartedRegistration;

    private readonly ConsoleLifetime consoleLifetime;

    public DebugWarmupLifetime
    (
        IOptions<ConsoleLifetimeOptions> options,
        IHostEnvironment environment,
        IHostApplicationLifetime applicationLifetime,
        IOptions<HostOptions> hostOptions,
        ILoggerFactory loggerFactory,
        ServerConfiguration serverConfiguration
    )
    {
        this.consoleLifetime = new ConsoleLifetime(options, environment, applicationLifetime, hostOptions, loggerFactory);
        this.ServerConfiguration = serverConfiguration;
        this.ApplicationLifetime = applicationLifetime;
    }

    public void OnApplicationStarted()
    {
        using HttpClient client = new();

        string url = ServerStatics.ServerType switch
        {
            ServerType.GameServer => this.ServerConfiguration.GameApiListenUrl,
            ServerType.Website => this.ServerConfiguration.WebsiteListenUrl,
            ServerType.Api => this.ServerConfiguration.ApiListenUrl,
            _ => throw new ArgumentOutOfRangeException(),
        };

        url = url.Replace("0.0.0.0", "127.0.0.1");

        Logger.Debug("Warming up Hot Reload...", LogArea.Startup);
        try
        {
            client.GetAsync(url).Wait();
        }
        catch(Exception e)
        {
            Logger.Debug("An error occurred while attempting to warm up hot reload. Initial page load will be delayed.", LogArea.Startup);
            Logger.Debug(e.ToDetailedException(), LogArea.Startup);
            return;
        }
        Logger.Success("Hot Reload is ready to go!", LogArea.Startup);
    }

    public Task StopAsync(CancellationToken cancellationToken) => this.consoleLifetime.StopAsync(cancellationToken);

    public Task WaitForStartAsync(CancellationToken cancellationToken)
    {
        this.applicationStartedRegistration = this.ApplicationLifetime.ApplicationStarted.Register((Action<object>)(_ => this.OnApplicationStarted()), this);

        return this.consoleLifetime.WaitForStartAsync(cancellationToken);
    }
}