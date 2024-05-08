using Content.Shared.Actions;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Shared.Aliens.Components;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent]
public sealed partial class AlienQueenComponent : Component
{
    [DataField("plasmaCostNode")]
    [ViewVariables(VVAccess.ReadWrite)]
    public float PlasmaCostEgg = 75f;

    /// <summary>
    /// The egg prototype to use.
    /// </summary>
    [DataField("eggPrototype", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string EggPrototype = "AlienEggGrowing";

    [DataField("eggAction", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string? EggAction = "ActionAlienEgg";

    [DataField("eggActionEntity")] public EntityUid? EggActionEntity;
}

public sealed partial class AlienEggActionEvent : InstantActionEvent { }
