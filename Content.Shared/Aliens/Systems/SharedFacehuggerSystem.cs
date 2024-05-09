using System.Linq;
using Content.Shared.Aliens.Components;
using Content.Shared.Inventory;
using Content.Shared.Inventory.Events;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;

namespace Content.Shared.Aliens.Systems;

/// <summary>
/// This handles...
/// </summary>
public sealed class SharedFacehuggerSystem : EntitySystem
{
    /// <inheritdoc/>
    [Dependency] private readonly EntityLookupSystem _lookup = default!;
    [Dependency] private readonly InventorySystem _inventory = default!;
    public override void Initialize()
    {
        SubscribeLocalEvent<FacehuggerComponent, GotEquippedEvent>(OnEquipped);
    }

    private void OnEquipped(EntityUid uid, FacehuggerComponent component, GotEquippedEvent args)
    {
        if (component.Active)
        {
            var inactive = Spawn(component.InactiveEntity);
            _inventory.TryEquip(args.Equipee, inactive, "mask");

            QueueDel(uid);
        }
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<FacehuggerComponent>();

        while (query.MoveNext(out var uid, out var alien))
        {
            foreach (var entity in _lookup.GetEntitiesInRange(uid, alien.Range)
                         .Where(entity => _inventory.HasSlot(entity, "mask")))
            {
                if(_inventory.CanAccess(uid, entity, uid) && EnsureComp<MobStateComponent>(uid).CurrentState == MobState.Alive)
                    _inventory.TryEquip(entity, uid, "mask");
            }
        }
    }
}
