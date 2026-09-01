namespace LBPUnion.ProjectLighthouse.Configuration.ConfigurationCategories;

public class AuthenticationConfiguration
{
    public bool RegistrationEnabled { get; set; } = true;
    public bool AutomaticAccountCreation { get; set; } = true;
    public bool VerifyTickets { get; set; } = true;

    public bool AllowRPCNSignup { get; set; } = true;

    public bool AllowPSNSignup { get; set; } = true;

    // Require client to authenticate with a user agent provided by the "Patchwork" security plugin
    public bool RequirePatchworkUserAgent { get; set; } = false;
    public int PatchworkMajorVersionMinimum { get; set; } = 1;
    public int PatchworkMinorVersionMinimum { get; set; } = 0;
    
}