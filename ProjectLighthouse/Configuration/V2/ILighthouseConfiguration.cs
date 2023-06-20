using YamlDotNet.Serialization;

namespace LBPUnion.ProjectLighthouse.Configuration.V2;

public interface ILighthouseConfiguration
{
    [YamlMember(Alias = "configVersionDoNotModifyOrYouWillBeSlapped", Order = -2)]
    public int ConfigVersion { get; set; }
}