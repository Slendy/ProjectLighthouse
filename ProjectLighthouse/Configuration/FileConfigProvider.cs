namespace LBPUnion.ProjectLighthouse.Configuration;

public class FileConfigProvider : ILighthouseConfigProvider
{
    public void SaveConfig(ConfigurationBase config, string serialized)
    {
        System.IO.File.WriteAllText(config.ConfigName, serialized);
    }

    public string LoadConfig(ConfigurationBase config) => System.IO.File.ReadAllText(config.ConfigName);
}