using Content.Shared.FixedPoint;
using Robust.Shared.GameStates;

namespace Content.Shared.Aliens.Components;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent, NetworkedComponent]
public sealed partial class PlasmaVesselComponent : Component
{
    /// <summary>
    /// The total amount of plasma the alien has.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public FixedPoint2 Plasma = 500;

    /// <summary>
    /// The entity's current max amount of essence. Can be increased
    /// through harvesting player souls.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField("maxPlasma")]
    public FixedPoint2 PlasmaRegenCap = 500;

    /// <summary>
    /// The amount of essence passively generated per second.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField("plasmaPerSecond")]
    public FixedPoint2 PlasmaPerSecond = 1f;

    [ViewVariables]
    public float Accumulator = 0;
}
