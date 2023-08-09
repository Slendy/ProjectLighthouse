using System.Collections.Generic;
using Newtonsoft.Json;

namespace LBPUnion.ProjectLighthouse.Types.Matchmaking;

public abstract class MatchResponse
{
    [JsonIgnore]
    public StatusCodeResponse StatusCode { get; set; }
}

public class StatusCodeResponse
{
    [JsonProperty]
    public int StatusCode { get; set; }

    private List<object> Items
    {
        get =>
            new()
            {
                this,
            };
    }
}