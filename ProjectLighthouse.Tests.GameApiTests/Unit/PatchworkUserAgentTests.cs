using LBPUnion.ProjectLighthouse.Servers.GameServer.Helpers;
using Xunit;

namespace ProjectLighthouse.Tests.GameApiTests.Unit;

[Trait("Category", "Unit")]
public class PatchworkUserAgentTests
{

    [Fact]
    public void CanValidatePatchworkUserAgents()
    {
        var validUserAgents = new[]
        {
            ("PatchworkLBP1 1.0", 1, 0, true),
            ("PatchworkLBP2 2.0 NK", 2, 0, false),
            ("PatchworkLBP3 3.0", 3, 0, true),
            ("PatchworkLBPV 4.0 NK", 4, 0, false),
            ("PatchworkLBP1 1.5", 1, 5, true),
        };

        string[] invalidUserAgents =
        {
            // Matching
            "patchworklbp1 1.0", // Case sensitive
            "ptchwrklbp1 1.0", // Misspelled
            "PatchworkLBP1 1", // Missing major/minor
            "PatchworkLBP1 1.100000", // Major/minor too long

            // Data
            "PatchworkLBP1 0.5", // Version number too low
            "PatchworkLBP1 A.0", // Int cannot be parsed
        };

        foreach (var (userAgent, expectedMajor, expectedMinor, expectedHasKey) in validUserAgents)
        {
            var valid = PatchworkHelper.IsValidPatchworkUserAgent(
                userAgent,
                out var major,
                out var minor,
                out var hasKey);

            Assert.True(valid);
            Assert.Equal(expectedMajor, major);
            Assert.Equal(expectedMinor, minor);
            Assert.Equal(expectedHasKey, hasKey);
        }

        foreach (string userAgent in invalidUserAgents)
        {
            var valid = PatchworkHelper.IsValidPatchworkUserAgent(userAgent, out _, out _, out _);

            Assert.False(valid);
        }
    }
}