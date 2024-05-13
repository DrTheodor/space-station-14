using System.Threading;
using Content.Shared.Aliens.Components;
using Content.Shared.Body.Events;
using Content.Shared.Body.Systems;

namespace Content.Shared.Aliens.Systems;

/// <summary>
/// This handles...
/// </summary>
public sealed class SharedLarvaOrganSystem : EntitySystem
{
    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<LarvaOrganComponent, OrganRemovedFromBodyEvent>(OnLarvaRemoved);
    }


    private void OnLarvaRemoved(EntityUid uid, LarvaOrganComponent component, OrganRemovedFromBodyEvent args)
    {
        RemComp<AlienInfectedComponent>(args.OldBody);
        Spawn(component.EntityProduced, Transform(args.OldBody).Coordinates);
    }
}
