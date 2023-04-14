using System.Collections.Generic;
using JetBrains.Annotations;
using LBPUnion.ProjectLighthouse.Types.Synchronization;
using YamlDotNet.Serialization;

namespace LBPUnion.ProjectLighthouse.Configuration;

public enum FilterMode
{
    None,
    Asterisks,
    Random,
    Furry,
}

public class CensorConfiguration : ConfigurationBase
{

    public CensorConfiguration(ILighthouseConfigProvider configProvider, ILighthouseMutex mutex) : base(configProvider, mutex)
    { }

    // HEY, YOU!
    // THIS VALUE MUST BE INCREMENTED FOR EVERY CONFIG CHANGE!
    //
    // This is so Lighthouse can properly identify outdated configurations and update them with newer settings accordingly.
    // If you are modifying anything here, this value MUST be incremented.
    // Thanks for listening~
    public override int ConfigVersion { get; set; } = 1;
    public override string ConfigName { get; set; } = "censor.yml";

    public FilterMode UserInputFilterMode { get; set; } = FilterMode.None;

    public List<string> FilteredWordList { get; set; } = new()
    {
        "cunt",
        "fag",
        "faggot",
        "horny",
        "kook",
        "kys",
        "loli",
        "nigga",
        "nigger",
        "penis",
        "pussy",
        "retard",
        "retarded",
        "vagina",
        "vore",
        "restitched",
        "h4h",
    };

}