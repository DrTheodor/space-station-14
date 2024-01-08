using System.Linq;
using Content.Shared.Actions;
using Content.Shared.Changeling.Components;
using Content.Shared.Humanoid;
using Content.Shared.IdentityManagement.Components;
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


    }

    private void OnStartup(EntityUid uid, ChangelingComponent component, ComponentStartup args)
    {
        _action.AddAction(uid, ref component.ActionEvolveEntity, ChangelingEvolveId);
        _action.AddAction(uid, ref component.ActionTransformEntity, ChangelingTransformId);
    }

}

