using Content.Shared.FixedPoint;

namespace Content.Shared.Changeling.Components;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent]
public sealed partial class ChangelingComponent : Component
{
    [DataField] public EntityUid? Action;
}


