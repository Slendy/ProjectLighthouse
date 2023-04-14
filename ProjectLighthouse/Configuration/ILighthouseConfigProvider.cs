namespace LBPUnion.ProjectLighthouse.Configuration;

public interface ILighthouseConfigProvider
{

    public void SaveConfig(ConfigurationBase config, string serialized);

    public string LoadConfig(ConfigurationBase config);

}