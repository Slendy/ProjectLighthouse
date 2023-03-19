using LBPUnion.ProjectLighthouse.Types.Misc;

namespace LBPUnion.ProjectLighthouse.Servers.Website.Types;

public enum LocationType
{
    Profile,
    Level,
}

public class UserEarth
{

    public Dictionary<Location, LocationType> Locations = new();

    public Location GetProfileLocation() => this.Locations.First(l => l.Value == LocationType.Profile).Key;

    public void AddUserLocation(Location l) => this.Locations.Add(l, LocationType.Profile);

    public void AddLevelLocation(Location l) => this.Locations.Add(l, LocationType.Level);

}