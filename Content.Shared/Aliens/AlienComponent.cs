using Content.Shared.Actions;
using Content.Shared.Chemistry.Reagent;
using Content.Shared.Devour;
using Content.Shared.Tag;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Containers;
using Robust.Shared.GameStates;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype.Set;

namespace Content.Shared.Aliens;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent, NetworkedComponent]
[AutoGenerateComponentState(true)]
public sealed partial class AlienComponent : Component
{
    // Actions
    [DataField]
    public EntProtoId ToggleLightingAction = "ActionToggleLightingAlien";

    [DataField, AutoNetworkedField]
    public EntityUid? ToggleLightingActionEntity;

        [DataField("devourAction", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string? DevourAction = "ActionDevour";

}

public sealed partial class ToggleLightingAlienActionEvent : InstantActionEvent { }
