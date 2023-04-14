using System;
using System.Threading.Tasks;
using JetBrains.Annotations;
using LBPUnion.ProjectLighthouse.Logging;

namespace LBPUnion.ProjectLighthouse.Types.Maintenance;

[UsedImplicitly(ImplicitUseTargetFlags.WithInheritors)]
public interface ICommand
{
    public string FirstAlias => this.Aliases()[0];
    public Task Run(IServiceProvider serviceProvider, string[] args, Logger logger);

    public string Name();

    public string[] Aliases();

    public string Arguments();

    public int RequiredArgs();
}