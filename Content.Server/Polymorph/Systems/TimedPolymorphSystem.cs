using Content.Server.Mind;
using Content.Server.Polymorph.Components;
using Content.Shared.Zombies;
using Robust.Shared.Prototypes;
using Robust.Shared.Spawners;
using Robust.Shared.Timing;

namespace Content.Server.Polymorph.Systems;

/// <summary>
/// This handles polymorphing entity after time
/// </summary>
public sealed class TimedPolymorphSystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly PolymorphSystem _polymorph = default!;
    [Dependency] private readonly IPrototypeManager _protoManager= default!;
    [Dependency] private readonly MindSystem _mindSystem = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();
        UpdatesOutsidePrediction = true;
        // SubscribeLocalEvent<TimedPolymorphComponent, TimedPolymorphComponent.TimedPolymorphEvent>(OnTimedPolymorph);
        SubscribeLocalEvent<TimedPolymorphComponent, TimedDespawnEvent>(OnTimedDespawn);
    }

    private void OnTimedDespawn(EntityUid uid, TimedPolymorphComponent component,
        TimedDespawnEvent args)
    {
        if (!_timing.IsFirstTimePredicted)
            return;

        var coords = Transform(uid).Coordinates;
        var newEntity = EntityManager.SpawnAtPosition(component.EntityPrototype, coords);

        // Move the mind if there is one and it's supposed to be transferred
        if (_mindSystem.TryGetMind(uid, out var mindId, out var mind))
            _mindSystem.TransferTo(mindId, newEntity, mind: mind);
    }

    /*
    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        if (!_timing.IsFirstTimePredicted)
            return;

        var query = EntityQueryEnumerator<TimedPolymorphComponent>();

        while (query.MoveNext(out var uid, out var comp))
        {
            comp.PolymorphTime -= frameTime;

            if (comp.PolymorphTime <= 0)
            {
                var ev = new TimedPolymorphComponent.TimedPolymorphEvent();
                RaiseLocalEvent(uid, ref ev);
            }
        }
    } */
}
