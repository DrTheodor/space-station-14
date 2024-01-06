using Content.Server.Store.Components;
using Content.Server.Store.Systems;
using Content.Shared.Changeling;
using Content.Shared.Changeling.Components;
using Robust.Shared.Prototypes;

namespace Content.Server.Changeling;

/// <summary>
/// This handles...
/// </summary>


public sealed class ChangelingSystem : EntitySystem
{

    [Dependency] private readonly StoreSystem _store = default!;
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
        Console.WriteLine("Sting Extract");
    }
}
