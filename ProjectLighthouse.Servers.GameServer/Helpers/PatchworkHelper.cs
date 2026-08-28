using LBPUnion.ProjectLighthouse.Configuration;
using System.Text.RegularExpressions;

namespace LBPUnion.ProjectLighthouse.Servers.GameServer.Helpers;

public static partial class PatchworkHelper
{
    private static readonly int requiredMajor = ServerConfiguration.Instance.Authentication.PatchworkMajorVersionMinimum;
    private static readonly int requiredMinor = ServerConfiguration.Instance.Authentication.PatchworkMinorVersionMinimum;

    // TODO: Ideally move off a regex at some point
    [GeneratedRegex(@"^PatchworkLBP[123V] (\d{1,5})\.(\d{1,5})?\s*(?<NK>NK)?\b")]
    private static partial Regex PatchworkUserAgentRegex();

    public static bool IsVersionGreaterThanOrEqualTo(int targetMajor, int targetMinor, int major, int minor)
        => (major > targetMajor) || (major == targetMajor && minor >= targetMinor);

    public static bool IsValidPatchworkUserAgent(string userAgent, out int? major, out int? minor, out bool? hasKey)
    {
        major = null;
        minor = null;
        hasKey = null;

        Match result = PatchworkUserAgentRegex().Match(userAgent);
        if (!result.Success) return false;

        if (!int.TryParse(result.Groups[1].Value, out int majorVal) || 
            !int.TryParse(result.Groups[2].Value, out int minorVal))
        {
            return false;
        }

        major = majorVal;
        minor = minorVal;

        // Patchwork 2.0 implements a specifier in the user agent to tell the server it does not have a join key set
        hasKey = result.Groups[3].Success ? false : true;

        return IsVersionGreaterThanOrEqualTo(requiredMajor, requiredMinor, major.Value, minor.Value);
    }
}
