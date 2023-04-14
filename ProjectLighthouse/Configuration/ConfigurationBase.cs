#nullable enable
using System;
using System.IO;
using System.Reflection;
using LBPUnion.ProjectLighthouse.Logging;
using LBPUnion.ProjectLighthouse.Types.Logging;
using LBPUnion.ProjectLighthouse.Types.Synchronization;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
// ReSharper disable VirtualMemberCallInConstructor

namespace LBPUnion.ProjectLighthouse.Configuration;

[Serializable]
public abstract class ConfigurationBase
{
    // Global mutex for synchronizing processes so that only one process can read a config at a time
    // Mostly useful for migrations so only one server will try to rewrite the config file
    private readonly ILighthouseMutex configFileMutex;

    private readonly ILighthouseConfigProvider configProvider;

    [YamlIgnore]
    public abstract string ConfigName { get; set; }

    [YamlMember(Alias = "configVersionDoNotModifyOrYouWillBeSlapped", Order = -2)]
    public abstract int ConfigVersion { get; set; }

    public readonly int LatestConfigVersion;


    protected ConfigurationBase(ILighthouseConfigProvider configProvider, ILighthouseMutex mutex)
    {
        this.LatestConfigVersion = this.ConfigVersion;
        this.configProvider = configProvider;
        this.configFileMutex = mutex;
    }

    protected ConfigurationBase()
    {

    }

    // internal ConfigurationBase()
    // {
    //     // Trim ConfigName by 4 to remove the .yml
    //     string mutexName = $"Global\\LighthouseConfig-{this.ConfigName[..^4]}";
    //     
    //     _configFileMutex = new Mutex(false, mutexName);
    //     
    //     this.loadStoredConfig();
    //     
    //     if (!this.ConfigReloading) return;
    //     
    //     _fileWatcher = new FileSystemWatcher
    //     {
    //         Path = Environment.CurrentDirectory,
    //         Filter = this.ConfigName,
    //         NotifyFilter = NotifyFilters.LastWrite, // only watch for writes to config file
    //     };
    //     
    //     _fileWatcher.Changed += this.onConfigChanged; // add event handler
    //     
    //     _fileWatcher.EnableRaisingEvents = true; // begin watching
    // }

    // internal void onConfigChanged(object sender, FileSystemEventArgs e)
    // {
    //     if (_fileWatcher == null) return;
    //     try
    //     {
    //         _fileWatcher.EnableRaisingEvents = false;
    //         Debug.Assert(e.Name == this.ConfigName);
    //         Logger.Info("Configuration file modified, reloading config...", LogArea.Config);
    //         Logger.Warn("Some changes may not apply; they will require a restart of Lighthouse.", LogArea.Config);
    //
    //         this.loadStoredConfig();
    //
    //         Logger.Success("Successfully reloaded the configuration!", LogArea.Config);
    //     }
    //     finally
    //     {
    //         _fileWatcher.EnableRaisingEvents = true;
    //     }
    // }

    public void LoadConfig()
    {
        try
        {
            this.configFileMutex.WaitOne();

            ConfigurationBase? storedConfig;

            if (File.Exists(this.ConfigName) && (storedConfig = this.fromFile(this.ConfigName)) != null)
            {
                if (storedConfig.ConfigVersion < this.LatestConfigVersion)
                {
                    Logger.Info($"Upgrading config file from version {storedConfig.ConfigVersion} to version {this.LatestConfigVersion}", LogArea.Config);
                    File.Copy(this.ConfigName, this.ConfigName + ".v" + storedConfig.ConfigVersion);
                    this.LoadConfig(storedConfig);
                    this.ConfigVersion = this.LatestConfigVersion;
                    this.WriteConfig();
                }
                else
                {
                    this.LoadConfig(storedConfig);
                }
            }
            else if (!File.Exists(this.ConfigName))
            {
                this.WriteConfig();
            }
        }
        finally
        {
            this.configFileMutex.ReleaseMutex();
        }
    }

    /// <summary>
    /// Uses reflection to set all values of this class to the values of another class
    /// </summary>
    /// <param name="otherConfig">The config to be loaded</param>
    private void LoadConfig(ConfigurationBase otherConfig)
    {
        foreach (PropertyInfo propertyInfo in otherConfig.GetType().GetProperties())
        {
            object? value = propertyInfo.GetValue(otherConfig);
            PropertyInfo? local = this.GetType().GetProperty(propertyInfo.Name);
            if (value == null || local == null || Attribute.IsDefined(local, typeof(YamlIgnoreAttribute)))
            {
                continue;
            }

            // Expand environment variables in strings. Format is windows-like (%ENV_NAME%)
            if (propertyInfo.PropertyType == typeof(string)) value = Environment.ExpandEnvironmentVariables((string)value);

            local.SetValue(this, value);
        }
    }

    private ConfigurationBase? fromFile(string path)
    {
        IDeserializer deserializer = new DeserializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance)
            .IgnoreUnmatchedProperties()
            .Build();

        try
        {
            string text = this.configProvider.LoadConfig(this);

            if (text.StartsWith("configVersionDoNotModifyOrYouWillBeSlapped"))
                return deserializer.Deserialize<ConfigurationBase>(text);
        }
        catch (Exception e)
        {
            Logger.Error($"Error while deserializing config: {e}", LogArea.Config);
            return null;
        }

        Logger.Error($"Unable to load config for {this.GetType().Name}", LogArea.Config);
        return null;
    }

    private string SerializeConfig() => new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build().Serialize(this);

    private void WriteConfig() => this.configProvider.SaveConfig(this, this.SerializeConfig());

    public void Dispose()
    {
        this.configFileMutex.Dispose();
        // _fileWatcher?.Dispose();
    }

}