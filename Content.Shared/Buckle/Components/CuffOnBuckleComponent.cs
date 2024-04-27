using Robust.Shared.Prototypes;

namespace Content.Shared.Buckle.Components;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent]
public sealed partial class CuffOnBuckleComponent : Component
{
    /// <summary>
    ///     Cuff prototype
    /// </summary>
    [DataField, ViewVariables(VVAccess.ReadWrite)]
    public EntProtoId? CuffsPrototype;
}
