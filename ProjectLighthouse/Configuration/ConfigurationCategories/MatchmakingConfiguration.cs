namespace LBPUnion.ProjectLighthouse.Configuration.ConfigurationCategories;

public class MatchmakingConfiguration
{
    public bool MatchmakingEnabled { get; set; } = true;

    // If `MatchmakingEnabled` is false, this option will still allow matchmaking for Patchwork users
    public bool PatchworkMatchmakingEnabled { get; set; } = false;
}