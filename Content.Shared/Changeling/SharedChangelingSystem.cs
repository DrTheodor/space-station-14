using System.Linq;
using Content.Shared.Actions;
using Content.Shared.Changeling.Components;
using Content.Shared.Humanoid;
using Content.Shared.Popups;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;

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
        SubscribeLocalEvent<ChangelingComponent, ComponentStartup>(OnStartup);

        SubscribeLocalEvent<ChangelingComponent, ChangelingTransformActionEvent>(OnTransform);
    }

    private void OnStartup(EntityUid uid, ChangelingComponent component, ComponentStartup args)
    {
        _action.AddAction(uid, ref component.ActionEvolveEntity, ChangelingEvolveId);
        _action.AddAction(uid, ref component.ActionTransformEntity, ChangelingTransformId);
    }



    private void OnTransform(EntityUid uid, ChangelingComponent component, ChangelingTransformActionEvent args)
    {
        if (component.Chemicals >= 5)
        {
            RemComp<HumanoidAppearanceComponent>(uid);

            AddComp(uid, component.DnaBank.Last().Value);
        }
        else if (component.Chemicals < 5)
        {
            _popup.PopupEntity(Loc.GetString("changeling-not-enough-chemicals-popup"), uid, uid);
        }
    }
}

