﻿using Content.Shared.Actions;
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
public sealed class SharedResinSpinnerSystem : EntitySystem
{
    /// <inheritdoc/>
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfterSystem = default!;
    [Dependency] private readonly SharedPopupSystem _popupSystem = default!;
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ResinSpinnerComponent, ComponentInit>(OnComponentInit);

        SubscribeLocalEvent<ResinSpinnerComponent, ResinWallActionEvent>(OnWall);
        SubscribeLocalEvent<ResinSpinnerComponent, ResinWindowActionEvent>(OnWindow);
    }

    private void OnComponentInit(EntityUid uid, ResinSpinnerComponent component, ComponentInit args)
    {
        _actionsSystem.AddAction(uid, ref component.ResinWallActionEntity, component.ResinWallAction, uid);
        _actionsSystem.AddAction(uid, ref component.ResinWindowActionEntity, component.ResinWindowAction, uid);
    }

    private void OnWall(EntityUid uid, ResinSpinnerComponent component, ResinWallActionEvent args)
    {
        OnStructureMaking(uid, component.PlasmaCostWall, component.ProductionLengthWall, component, new ResinWallDoAfterEvent());
    }

    private void OnWindow(EntityUid uid, ResinSpinnerComponent component, ResinWindowActionEvent args)
    {
        OnStructureMaking(uid, component.PlasmaCostWindow, component.ProductionLengthWindow, component, new ResinWindowDoAfterEvent());
    }

    private void OnStructureMaking(EntityUid uid, float cost, float productionLength, ResinSpinnerComponent component, DoAfterEvent doAfterEvent)
    {
        if (TryComp<PlasmaVesselComponent>(uid, out var plasmaComp)
            && plasmaComp.Plasma < cost)
        {
            _popupSystem.PopupClient(Loc.GetString(component.PopupText), uid, uid);
            return;
        }

        var doAfter = new DoAfterArgs(EntityManager, uid, productionLength, doAfterEvent, uid)
        { // I'm not sure if more things should be put here, but imo ideally it should probably be set in the component/YAML. Not sure if this is currently possible.
            BlockDuplicate = true,
            BreakOnDamage = true,
            CancelDuplicate = true,
            BreakOnMove = true
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
