using System.Linq;
using Content.Server.Store.Components;
using Content.Server.Store.Systems;
using Content.Shared.Alert;
using Content.Shared.Changeling;
using Content.Shared.Changeling.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Humanoid;
using Content.Shared.Popups;
using Content.Shared.Revenant.Components;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.Manager;

namespace Content.Server.Changeling;

/// <summary>
/// This handles...
/// </summary>


public sealed class ChangelingSystem : EntitySystem
{

    [Dependency] private readonly StoreSystem _store = default!;
    [Dependency] private readonly SharedPopupSystem _popup = default!;
    [Dependency] private readonly ISerializationManager _serializationManager = default!;
    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ChangelingComponent, ChangelingEvolveActionEvent>(OnEvolve);
        SubscribeLocalEvent<ChangelingComponent, ChangelingStingExtractActionEvent>(OnStingExtract);
    }

    private void OnEvolve(EntityUid uid, ChangelingComponent component, ChangelingEvolveActionEvent args)
    {
        if (!TryComp<StoreComponent>(uid, out var store))
            return;
        _store.ToggleUi(uid, uid, store);
    }


    private void OnStingExtract(EntityUid uid, ChangelingComponent component, ChangelingStingExtractActionEvent args)
    {

        if (HasComp<HumanoidAppearanceComponent>(args.Target) && component.Chemicals >= 25)
        {
            _popup.PopupEntity(Loc.GetString("changeling-sting-extract-popup") + Name(args.Target), uid, uid);

            component.DnaBank[Name(args.Target)] = _serializationManager.CreateCopy(Comp<HumanoidAppearanceComponent>(args.Target), notNullableOverride: true);
            ChangeChemicalsAmount(uid, -25, component);
        }
        else if (component.Chemicals < 25)
        {
            _popup.PopupEntity(Loc.GetString("changeling-not-enough-chemicals-popup"), uid, uid);
        }
    }

    public bool ChangeChemicalsAmount(EntityUid uid, FixedPoint2 amount, ChangelingComponent? component = null, bool regenCap = true)
    {
        if (!Resolve(uid, ref component))
            return false;

        if (component.Chemicals + amount < 0)
            return false;

        component.Chemicals += amount;

        if (regenCap)
            FixedPoint2.Min(component.Chemicals, component.ChemicalsRegenCap);

        if (TryComp<StoreComponent>(uid, out var store))
            _store.UpdateUserInterface(uid, uid, store);

        return true;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<ChangelingComponent>();
        while (query.MoveNext(out var uid, out var ling))
        {
            ling.Accumulator += frameTime;

            if (ling.Accumulator <= 1)
                continue;
            ling.Accumulator -= 1;

            if (ling.Chemicals < ling.ChemicalsRegenCap)
            {
                ChangeChemicalsAmount(uid, ling.ChemicalsPerSecond, ling, regenCap: true);
            }
        }
    }
}
