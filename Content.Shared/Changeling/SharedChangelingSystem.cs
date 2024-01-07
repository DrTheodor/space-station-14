using Content.Shared.Actions;
using Content.Shared.Changeling.Components;
using Robust.Shared.Prototypes;

namespace Content.Shared.Changeling;

/// <summary>
/// This handles...
/// </summary>
public sealed class SharedChangelingSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _action = default!;
    [ValidatePrototypeId<EntityPrototype>]
    private const string ChangelingEvolveId = "ActionChangelingEvolve";
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<ChangelingComponent, MapInitEvent>(OnInit);
    }
    private void OnInit(EntityUid uid, ChangelingComponent component, MapInitEvent args)
    {
        _action.AddAction(uid, ref component.Action, ChangelingEvolveId);
    }
}
public sealed partial class ChangelingEvolveActionEvent : InstantActionEvent
{
}

public sealed partial class ChangelingStingExtractActionEvent : EntityTargetActionEvent
{
}

