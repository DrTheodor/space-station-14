using System.Linq;
using Content.Server.Administration.Systems;
using Content.Server.Forensics;
using Content.Server.Store.Components;
using Content.Server.Store.Systems;
using Content.Shared.Alert;
using Content.Shared.Changeling;
using Content.Shared.Changeling.Components;
using Content.Shared.FixedPoint;
using Content.Shared.Humanoid;
using Content.Shared.IdentityManagement.Components;
using Content.Shared.Mind;
using Content.Shared.Popups;
using Content.Shared.Revenant.Components;
using Microsoft.CodeAnalysis.Rename;
using Robust.Server.Player;
using Robust.Shared.Player;
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
    [Dependency] private readonly IEntityManager _entManager = default!;
    [Dependency] private readonly IPlayerManager _playerManager = default!;
    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ChangelingComponent, ChangelingEvolveActionEvent>(OnEvolve);
        SubscribeLocalEvent<ChangelingComponent, ChangelingStingExtractActionEvent>(OnStingExtract);
        SubscribeLocalEvent<ChangelingComponent, ChangelingTransformActionEvent>(OnTransform);
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
            AddDna(component, args.Target);

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

    public void AddDna(ChangelingComponent component, EntityUid target)
    {
        component.DnaBank[Name(target)] =
            [
                _serializationManager.CreateCopy(Comp<HumanoidAppearanceComponent>(target), notNullableOverride: true),
                _serializationManager.CreateCopy(Comp<DnaComponent>(target), notNullableOverride: true),
                _serializationManager.CreateCopy(Comp<FingerprintComponent>(target), notNullableOverride: true)
            ];
    }

    private void OnTransform(EntityUid uid, ChangelingComponent component, ChangelingTransformActionEvent args)
    {
        if (component.Chemicals >= 5)
        {
            RemComp<HumanoidAppearanceComponent>(uid);
            RemComp<DnaComponent>(uid);
            RemComp<FingerprintComponent>(uid);

            AddComp(uid, component.DnaBank.Last().Value[0]);
            AddComp(uid, component.DnaBank.Last().Value[1]);
            AddComp(uid, component.DnaBank.Last().Value[2]);

            RenameLing(component.DnaBank.Last().Key, uid);
        }
        else if (component.Chemicals < 5)
        {
            _popup.PopupEntity(Loc.GetString("changeling-not-enough-chemicals-popup"), uid, uid);
        }
    }

    public void RenameLing(string newName, EntityUid uid)
    {

        var name = newName;

        // Metadata
        var metadata = _entManager.GetComponent<MetaDataComponent>(uid);
        var oldName = metadata.EntityName;
        _entManager.System<MetaDataSystem>().SetEntityName(uid, name, metadata);

        var minds = _entManager.System<SharedMindSystem>();

        if (minds.TryGetMind(uid, out var mindId, out var mind))
        {
            // Mind
            mind.CharacterName = name;
            _entManager.Dirty(mindId, mind);
        }
        // Admin overlay name change
        /*
        if (_entManager.TrySystem<AdminSystem>(out var adminSystem)
            && _entManager.TryGetComponent<ActorComponent>(uid, out var actorComp))
        {
            adminSystem.UpdatePlayerList(actorComp.PlayerSession);
        } */
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
