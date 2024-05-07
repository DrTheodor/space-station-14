using Content.Server.Polymorph.Systems;
using Content.Server.Popups;
using Content.Shared.Actions;
using Content.Shared.Aliens.Components;
using Content.Shared.Aliens.Systems;
using Content.Shared.Polymorph;
using Robust.Shared.Prototypes;

namespace Content.Server.Aliens.Systems;

/// <summary>
/// This handles...
/// </summary>
public sealed class PraetorianEvolutionSystem : EntitySystem
{
    [Dependency] private readonly SharedActionsSystem _actionsSystem = default!;
    [Dependency] private readonly AlienEvolutionSystem _alienEvolution = default!;
    [Dependency] private readonly PopupSystem _popup = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PraetorianEvolutionComponent, ComponentInit>(OnComponentInit);

        SubscribeLocalEvent<PraetorianEvolutionComponent, AlienPraetorianEvolveActionEvent>(OnEvolvePraetorian);
    }

    private void OnComponentInit(EntityUid uid, PraetorianEvolutionComponent component, ComponentInit args)
    {

        _actionsSystem.AddAction(uid, ref component.PraetorianEvolutionActionEntity, component.PraetorianEvolutionAction, uid);
    }

    private void OnEvolvePraetorian(EntityUid uid, PraetorianEvolutionComponent component, AlienPraetorianEvolveActionEvent args)
    {
        if (TryComp<PlasmaVesselComponent>(uid, out var plasmaComp)
            && plasmaComp.Plasma <= component.PlasmaCost)
        {
            _popup.PopupClient(Loc.GetString("alien-action-fail-plasma"), uid, uid);
            return;
        }
        _alienEvolution.Evolve(uid, component.PraetorianPolymorphPrototype);
    }
}


