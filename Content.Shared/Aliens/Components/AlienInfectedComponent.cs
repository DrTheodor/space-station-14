using System.Threading;
using Robust.Shared.Prototypes;

namespace Content.Shared.Aliens.Components;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent]
public sealed partial class AlienInfectedComponent : Component
{
    public CancellationTokenSource? TokenSource;

    [DataField]
    public float GrowTime = 330f;

    [ViewVariables(VVAccess.ReadWrite)]
    public EntProtoId EntityProduced = "MobAlienLarva";
}
