using Content.Server.Aliens.Components;
using Content.Server.Polymorph.Components;
using Content.Server.Polymorph.Systems;
using Content.Server.Stunnable;
using Content.Shared.Aliens.Components;
using Content.Shared.IdentityManagement.Components;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using AlienInfectedComponent = Content.Shared.Aliens.Components.AlienInfectedComponent;

namespace Content.Server.Aliens.Systems;

/// <summary>
/// This handles...
/// </summary>
public sealed class FacehuggerSystem : EntitySystem
{
    /// <inheritdoc/>
    [Dependency] private readonly StunSystem _stun = default!;
    [Dependency] private readonly InventorySystem _inventory = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<FacehuggerComponent, GotEquippedEvent>(OnEquipped);
        SubscribeLocalEvent<FacehuggerComponent, ComponentShutdown>(OnShutdown);
    }

    public void OnEquipped(EntityUid uid, FacehuggerComponent component, GotEquippedEvent args)
    {

        if(!HasComp<AlienInfectedComponent>(args.Equipee))
            AddComp<AlienInfectedComponent>(args.Equipee);
        _stun.TryParalyze(args.Equipee, TimeSpan.FromSeconds(25), false);
        RemComp<FacehuggerComponent>(uid);
    }

    public void OnShutdown(EntityUid uid, FacehuggerComponent component, ComponentShutdown args)
    {
        var polymorph = AddComp<TimedPolymorphComponent>(uid);
        polymorph.PolymorphTime = 0.1f;
        polymorph.PolymorphPrototype = component.FacehuggerPolymorphPrototype;
    }
}
