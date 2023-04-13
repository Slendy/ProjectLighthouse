#nullable enable
using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using LBPUnion.ProjectLighthouse.Logging;
using LBPUnion.ProjectLighthouse.Types.Logging;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace LBPUnion.ProjectLighthouse.Configuration;

[Serializable]
public abstract class ConfigurationBase
{
    // Global mutex for synchronizing processes so that only one process can read a config at a time
    // Mostly useful for migrations so only one server will try to rewrite the config file
    // private static Mutex? _configFileMutex;

    private readonly Lazy<Mutex?> configFileMutex;

    [YamlIgnore]
    public abstract string ConfigName { get; set; }

    [YamlMember(Alias = "configVersionDoNotModifyOrYouWillBeSlapped", Order = -2)]
    public abstract int ConfigVersion { get; set; }

    [YamlIgnore]
    // Used to indicate whether the config will be generated with a .configme extension or not
    public virtual bool NeedsConfiguration { get; set; } = true;

    [YamlMember(Order = -1)]
    public bool ConfigReloading { get; set; } = false;

    // Used to listen for changes to the config file
    private static FileSystemWatcher? _fileWatcher;

    protected ConfigurationBase(ILighthouseConfigProvider provider)
    {
        // Trim ConfigName by 4 to remove the .yml
        string mutexName = $"Global\\LighthouseConfig-{this.ConfigName[..^4]}";

        this.configFileMutex = new Lazy<Mutex?>(() => new Mutex(false, mutexName));
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

    internal void onConfigChanged(object sender, FileSystemEventArgs e)
    {
        if (_fileWatcher == null) return;
        try
        {
            _fileWatcher.EnableRaisingEvents = false;
            Debug.Assert(e.Name == this.ConfigName);
            Logger.Info("Configuration file modified, reloading config...", LogArea.Config);
            Logger.Warn("Some changes may not apply; they will require a restart of Lighthouse.", LogArea.Config);

            this.loadStoredConfig();

            Logger.Success("Successfully reloaded the configuration!", LogArea.Config);
        }
        finally
        {
            _fileWatcher.EnableRaisingEvents = true;
        }
    }

    private void loadStoredConfig()
    {
        try
        {
            this.configFileMutex.Value?.WaitOne();

            ConfigurationBase storedConfig;

            if (File.Exists(this.ConfigName) && (storedConfig = this.fromFile(this.ConfigName)) != null)
            {
                if (storedConfig.ConfigVersion < GetVersion())
                {
                    int newVersion = GetVersion();
                    Logger.Info($"Upgrading config file from version {storedConfig.ConfigVersion} to version {newVersion}", LogArea.Config);
                    File.Copy(this.ConfigName, this.ConfigName + ".v" + storedConfig.ConfigVersion);
                    this.loadConfig(storedConfig);
                    this.ConfigVersion = newVersion;
                    this.writeConfig(this.ConfigName);
                }
                else
                {
                    this.loadConfig(storedConfig);
                }
            }
            else if (!File.Exists(this.ConfigName))
            {
                if (this.NeedsConfiguration)
                {
                    Logger.Warn("The configuration file was not found. " +
                                "A blank configuration file has been created for you at " +
                                $"{Path.Combine(Environment.CurrentDirectory, this.ConfigName + ".configme")}",
                        LogArea.Config);
                    this.writeConfig(this.ConfigName + ".configme");
                    this.ConfigVersion = -1;
                }
                else
                {
                    this.writeConfig(this.ConfigName);
                }
            }
        }
        finally
        {
            this.configFileMutex.Value?.ReleaseMutex();
        }
    }

    /// <summary>
    /// Uses reflection to set all values of this class to the values of another class
    /// </summary>
    /// <param name="otherConfig">The config to be loaded</param>
    private void loadConfig(ConfigurationBase otherConfig)
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
            string text = File.ReadAllText(path);

            if (text.StartsWith("configVersionDoNotModifyOrYouWillBeSlapped"))
                return this.Deserialize(deserializer, text);
        }
        catch (Exception e)
        {
            Logger.Error($"Error while deserializing config: {e}", LogArea.Config);
            return null;
        }

        Logger.Error($"Unable to load config for {this.GetType().Name}", LogArea.Config);
        return null;
    }

    // public abstract ConfigurationBase<T> Deserialize(IDeserializer deserializer, string text);

    private string serializeConfig() => new SerializerBuilder().WithNamingConvention(CamelCaseNamingConvention.Instance).Build().Serialize(this);

    private void writeConfig(string path) => File.WriteAllText(path, this.serializeConfig());

    public void Dispose()
    {
        this.configFileMutex.Value?.Dispose();
        _fileWatcher?.Dispose();
    }

}