using Content.Shared.Actions;
using Content.Shared.Aliens.Components;
using Content.Shared.Coordinates.Helpers;
using Content.Shared.DoAfter;
using Content.Shared.Maps;
using Content.Shared.Mobs.Components;
using Content.Shared.Physics;
using Content.Shared.Popups;
using Robust.Shared.Serialization;

namespace Content.Shared.Aliens.Systems;

/// <summary>
/// This handles...
/// </summary>.
public sealed class SharedAlienDroneSystem : EntitySystem
{
    /// <inheritdoc/>
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfterSystem = default!;
    [Dependency] private readonly SharedPopupSystem _popupSystem = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AlienDroneComponent, ComponentInit>(OnComponentInit);

        SubscribeLocalEvent<AlienDroneComponent, ResinWallActionEvent>(OnWall);
        SubscribeLocalEvent<AlienDroneComponent, ResinWindowActionEvent>(OnWindow);
    }

    private void OnComponentInit(EntityUid uid, AlienDroneComponent component, ComponentInit args)
    {
        _actionsSystem.AddAction(uid, ref component.ResinWallActionEntity, component.ResinWallAction, uid);
        _actionsSystem.AddAction(uid, ref component.ResinWindowActionEntity, component.ResinWindowAction, uid);
    }

    private void OnWall(EntityUid uid, AlienDroneComponent component, ResinWallActionEvent args)
    {
        OnStructureMaking(uid, component.PlasmaCostWall, component.ProductionLengthWall, component, new ResinWallDoAfterEvent());
    }

    private void OnWindow(EntityUid uid, AlienDroneComponent component, ResinWindowActionEvent args)
    {
        OnStructureMaking(uid, component.PlasmaCostWindow, component.ProductionLengthWindow, component, new ResinWindowDoAfterEvent());
    }

    private void OnStructureMaking(EntityUid uid, float cost, float productionLength, AlienDroneComponent component, DoAfterEvent doAfterEvent)
    {
        if (TryComp<PlasmaVesselComponent>(uid, out var plasmaComp)
            && plasmaComp.Plasma < cost)
        {
            _popupSystem.PopupClient(Loc.GetString(component.PopupText), uid, uid);
            return;
        }

        var doAfter = new DoAfterArgs(EntityManager, uid, productionLength, doAfterEvent, uid)
        { // I'm not sure if more things should be put here, but imo ideally it should probably be set in the component/YAML. Not sure if this is currently possible.
            BreakOnUserMove = true,
            BlockDuplicate = true,
            BreakOnDamage = true,
            CancelDuplicate = true,
        };

        _doAfterSystem.TryStartDoAfter(doAfter);
    }
}

public sealed partial class ResinWallActionEvent : InstantActionEvent { }

public sealed partial class ResinWindowActionEvent : InstantActionEvent { }

/// <summary>
/// Is relayed at the end of the making structure.
/// </summary>
[Serializable, NetSerializable]
public sealed partial class ResinWallDoAfterEvent : SimpleDoAfterEvent { }

[Serializable, NetSerializable]

public sealed partial class ResinWindowDoAfterEvent : SimpleDoAfterEvent { }
