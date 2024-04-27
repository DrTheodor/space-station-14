using Content.Server.Actions;
using Content.Server.Animals.Components;
using Content.Server.Popups;
using Content.Shared.Aliens;
using Content.Shared.Devour;
using Content.Shared.Devour.Components;
using Content.Shared.DoAfter;
using Content.Shared.Item;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;
using Content.Shared.Tag;
using AlienComponent = Content.Shared.Aliens.Components.AlienComponent;

namespace Content.Server.Animals.Systems;

/// <summary>
/// This handles...
/// </summary>
public sealed class AlienSystem : EntitySystem
{
    [Dependency] private readonly TagSystem _tag = default!;
    [Dependency] private readonly PopupSystem _popup = default!;
    [Dependency] private readonly ActionsSystem _actions = default!;
    [Dependency] private readonly SharedDoAfterSystem _doAfterSystem = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<AlienComponent, PickupAttemptEvent>(OnPickup);
        SubscribeLocalEvent<AlienComponent, MapInitEvent>(OnMapInit);

    }

    private void OnMapInit(EntityUid uid, AlienComponent component, MapInitEvent args)
    {
        // _actions.AddAction(uid, ref component.ToggleLightingActionEntity, component.ToggleLightingAction);
    }

    private void OnPickup(EntityUid uid, AlienComponent component, PickupAttemptEvent args)
    {
        if (!_tag.HasTag(args.Item, "AlienItem"))
        {
            args.Cancel();
            _popup.PopupEntity(Loc.GetString("alien-pickup-item-fail"), uid);
        }
    }

}
