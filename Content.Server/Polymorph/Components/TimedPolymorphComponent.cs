using Content.Shared.Polymorph;
using Content.Shared.Whitelist;
using Robust.Shared.Audio;
using Robust.Shared.Prototypes;

namespace Content.Server.Polymorph.Components;

/// <summary>
/// This is used for polymorphing entity after time
/// </summary>
[RegisterComponent]
public sealed partial class TimedPolymorphComponent : Component
{
    [DataField(required: true)]
    public EntProtoId EntityPrototype = default!;

    [DataField]
    public SoundSpecifier Sound = new SoundPathSpecifier("/Audio/Magic/forcewall.ogg");

    [DataField(required: true)]
    public float PolymorphTime;

    [ByRefEvent]
    public readonly record struct TimedPolymorphEvent;
}
