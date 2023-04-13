namespace LBPUnion.ProjectLighthouse.Configuration;

public interface ILighthouseConfigProvider
{

    public void SaveConfig(ILighthouseConfig config);

    public ConfigurationBase LoadConfig(ILighthouseConfig config);

}