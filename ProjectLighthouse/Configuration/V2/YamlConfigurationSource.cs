using Microsoft.Extensions.Configuration;

namespace LBPUnion.ProjectLighthouse.Configuration.V2;

public class YamlConfigurationSource : FileConfigurationSource
{
    public ILighthouseConfiguration Config { get; set; }

    public override IConfigurationProvider Build(IConfigurationBuilder builder)
    {
        this.EnsureDefaults(builder);

        return new YamlConfigurationProvider(this);
    } 
}