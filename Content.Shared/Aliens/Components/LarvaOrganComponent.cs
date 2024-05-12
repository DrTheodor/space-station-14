using Robust.Shared.Prototypes;

namespace Content.Shared.Aliens.Components;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent]
public sealed partial class LarvaOrganComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite)]
    public EntProtoId EntityProduced = "MobAlienLarva";
}
