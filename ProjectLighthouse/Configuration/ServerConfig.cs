namespace LBPUnion.ProjectLighthouse.Configuration;

public class ServerConfig : ILighthouseConfig
{
    public ServerConfig(ILighthouseConfigProvider provider)
    {
        ServerConfiguration serverConfiguration = new ServerConfiguration();
        serverConfiguration.
        provider.LoadConfig(this);
    }

    public string ConfigName() => "lighthouse";


}