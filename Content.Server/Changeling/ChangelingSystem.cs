using Content.Shared.Changeling;
using Content.Shared.Changeling.Components;
using Robust.Shared.Prototypes;

namespace Content.Server.Changeling;

/// <summary>
/// This handles...
/// </summary>


public sealed class ChangelingSystem : EntitySystem
{

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ChangelingComponent, ChangelingEvolveActionEvent>(OnEvolve);
    }

    private void OnEvolve(EntityUid uid, ChangelingComponent component, ChangelingEvolveActionEvent args)
    {
        Console.WriteLine("Evolve");
    }
}
