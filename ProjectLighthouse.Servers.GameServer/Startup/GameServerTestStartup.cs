using LBPUnion.ProjectLighthouse.Configuration;
using LBPUnion.ProjectLighthouse.Middlewares;

namespace LBPUnion.ProjectLighthouse.Servers.GameServer.Startup;

public class GameServerTestStartup : GameServerStartup
{
    public GameServerTestStartup(IConfiguration configuration, ServerConfiguration serverConfiguration) : base(configuration, serverConfiguration)
    {}

    public override void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseMiddleware<FakeRemoteIPAddressMiddleware>();
        base.Configure(app, env);
    }
}