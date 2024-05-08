using Content.Server.Actions;
using Content.Server.Animals.Components;
using Content.Server.Popups;
using Content.Shared.Aliens.Components;
using Content.Shared.Aliens.Systems;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.Maps;
using Content.Shared.Physics;
using Content.Shared.Tag;
using Robust.Shared.Containers;
using Robust.Shared.Map;
using AlienEggActionEvent = Content.Shared.Aliens.Components.AlienEggActionEvent;
using AlienQueenComponent = Content.Shared.Aliens.Components.AlienQueenComponent;

namespace Content.Server.Aliens.Systems;

/// <summary>
/// This handles...
/// </summary>
public sealed class AlienQueenSystem : EntitySystem
{
    [Dependency] private readonly TagSystem _tag = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly ActionsSystem _actions = default!;
    [Dependency] private readonly EntityLookupSystem _lookupSystem = default!;
    [Dependency] private readonly TurfSystem _turf = default!;
    [Dependency] private readonly IMapManager _mapMan = default!;
    [Dependency] private readonly SharedContainerSystem _container = default!;
    [Dependency] private readonly SharedPlasmaVesselSystem _plasmaVessel = default!;
    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AlienQueenComponent, MapInitEvent>(OnMapInit);
        SubscribeLocalEvent<AlienQueenComponent, AlienEggActionEvent>(OnEgg);
    }

    private void OnMapInit(EntityUid uid, AlienQueenComponent component, MapInitEvent args)
    {
        _actions.AddAction(uid, ref component.EggActionEntity, component.EggAction);
    }

    private void OnEgg(EntityUid uid, AlienQueenComponent component, AlienEggActionEvent args)
    {
        if (TryComp<PlasmaVesselComponent>(uid, out var plasmaComp)
            && plasmaComp.Plasma < component.PlasmaCostEgg)
        {
            _popup.PopupClient(Loc.GetString(Loc.GetString("alien-action-fail-plasma")), uid, uid);
            return;
        }
        CreateStructure(uid, component);
        args.Handled = true;
    }

    public void CreateStructure(EntityUid uid, AlienQueenComponent component)
    {

        if (_container.IsEntityOrParentInContainer(uid))
            return;

        var xform = Transform(uid);
        // Get the tile in front of the drone
        var coords = xform.Coordinates.SnapToGrid(EntityManager, _mapMan);
        var tile = coords.GetTileRef(EntityManager, _mapMan);
        if (tile == null)
            return;

        // Check there are no walls there
        if (_turf.IsTileBlocked(tile.Value, CollisionGroup.Impassable))
        {
            _popup.PopupEntity(Loc.GetString("alien-create-structure-failed"), uid, uid);
            return;
        }

        foreach (var entity in _lookupSystem.GetEntitiesInRange(coords, 0.1f))
        {
            if (Prototype(entity) == null)
                continue;
            if (Prototype(entity)!.ID == component.EggPrototype)
                return;
        }

        _plasmaVessel.ChangePlasmaAmount(uid, -component.PlasmaCostEgg);
        Spawn(component.EggPrototype, _turf.GetTileCenter(tile.Value));
    }
}
