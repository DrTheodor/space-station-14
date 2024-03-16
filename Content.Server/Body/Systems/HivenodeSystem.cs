using Content.Server.Animals.Components;
using Content.Server.Body.Components;
using Content.Server.Popups;
using Content.Shared.Aliens;
using Content.Shared.Body.Part;

namespace Content.Server.Body.Systems;

/// <summary>
/// This handles...
/// </summary>
public sealed class HivenodeSystem : EntitySystem
{
    [Dependency] private readonly BodySystem _body = default!;
    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<HivenodeComponent, BodyPartAddedEvent>(HandleBodyPartAdded);
        SubscribeLocalEvent<HivenodeComponent, BodyPartRemovedEvent>(HandleBodyPartRemoved);
    }

    private void HandleBodyPartAdded(EntityUid uid, HivenodeComponent component, ref BodyPartAddedEvent args)
    {
        AddComp<AlienComponent>(_body.GetParentPartOrNull(uid)!.Value);
    }

    private void HandleBodyPartRemoved(EntityUid uid, HivenodeComponent component, ref BodyPartRemovedEvent args)
    {
        RemComp<AlienComponent>(_body.GetParentPartOrNull(uid)!.Value);
    }
}
