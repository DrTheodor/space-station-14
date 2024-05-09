using Robust.Shared.Prototypes;

namespace Content.Shared.Aliens.Components;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent]
public sealed partial class FacehuggerComponent : Component
{
    [DataField]
    public float Range = 3f;

    [DataField]
    public bool Active = true;

    [DataField]
    [ViewVariables(VVAccess.ReadWrite)]
    public EntProtoId InactiveEntity = "FacehuggerInactive";
}
