using System.Linq;
using Content.Shared.Actions;
using Content.Shared.Changeling.Components;
using Content.Shared.Humanoid;
using Content.Shared.Popups;
using Robust.Shared.Prototypes;

namespace Content.Shared.Changeling;

/// <summary>
/// This handles...
/// </summary>
public sealed class SharedChangelingSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _action = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;

    [ValidatePrototypeId<EntityPrototype>]
    private const string ChangelingEvolveId = "ActionChangelingEvolve";

    [ValidatePrototypeId<EntityPrototype>]
    private const string ChangelingTransformId = "ActionChangelingTransform";

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ChangelingComponent, MapInitEvent>(OnInit);

        SubscribeLocalEvent<ChangelingComponent, ChangelingTransformActionEvent>(OnTransform);
    }
    private void OnInit(EntityUid uid, ChangelingComponent component, MapInitEvent args)
    {
        _action.AddAction(uid, ref component.Action, ChangelingEvolveId);
        _action.AddAction(uid, ref component.Action, ChangelingTransformId);
    }

    private void OnTransform(EntityUid uid, ChangelingComponent component, ChangelingTransformActionEvent args)
    {
        if (component.Chemicals >= 5)
        {
            RemComp<HumanoidAppearanceComponent>(uid);
            AddComp(uid, component.DnaBank.ElementAt(-1).Value);
        }
        else if (component.Chemicals < 25)
        {
            _popup.PopupEntity(Loc.GetString("changeling-not-enough-chemicals-popup"), uid, uid);
        }
    }
}
public sealed partial class ChangelingEvolveActionEvent : InstantActionEvent
{
}

public sealed partial class ChangelingStingExtractActionEvent : EntityTargetActionEvent
{
}

public sealed partial class ChangelingTransformActionEvent : InstantActionEvent
{
}
