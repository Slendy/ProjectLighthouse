using System.Threading.Tasks;
using JetBrains.Annotations;
using LBPUnion.ProjectLighthouse.Database;

namespace LBPUnion.ProjectLighthouse.Types.Maintenance;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public interface IMaintenanceJob
{
    public Task Run(DatabaseContext database);

    public string Name();

    public string Description();
}