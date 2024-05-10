using System.Threading;
using Content.Server.Aliens.Components;
using Content.Shared.Damage;
using Content.Shared.Gibbing.Components;
using Content.Shared.Gibbing.Events;
using Content.Shared.Gibbing.Systems;
using Content.Shared.Mobs;
using AlienInfectedComponent = Content.Shared.Aliens.Components.AlienInfectedComponent;

namespace Content.Server.Aliens.Systems;

/// <summary>
/// This handles...
/// </summary>
public sealed class AlienInfectedSystem : EntitySystem
{
    /// <inheritdoc/>
    [Dependency] private readonly GibbingSystem _gibbing = default!;
    [Dependency] private readonly DamageableSystem _damageable = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<AlienInfectedComponent, ComponentInit>(OnComponentInit);
        SubscribeLocalEvent<AlienInfectedComponent, ComponentShutdown>(OnComponentShutdown);
        SubscribeLocalEvent<AlienInfectedComponent, MobStateChangedEvent>(OnMobState);
    }

    private void OnComponentInit(EntityUid uid, AlienInfectedComponent component, ComponentInit args)
    {
        component.TokenSource?.Cancel();
        component.TokenSource = new CancellationTokenSource();
        uid.SpawnRepeatingTimer(TimeSpan.FromSeconds(component.GrowTime), () => OnTimerFired(uid, component), component.TokenSource.Token);
    }

    private void OnTimerFired(EntityUid uid, AlienInfectedComponent component)
    {
        Spawn(component.EntityProduced, Transform(uid).Coordinates);
        var damage = new DamageSpecifier();
        damage.DamageDict.Add("Blunt", 400);
        _damageable.TryChangeDamage(uid, damage);
    }

    private void OnMobState(EntityUid uid, AlienInfectedComponent component, MobStateChangedEvent args)
    {
        if (args.NewMobState == MobState.Dead)
            RemComp<AlienInfectedComponent>(uid);
    }

    private void OnComponentShutdown(EntityUid uid, AlienInfectedComponent component, ComponentShutdown args)
    {
        component.TokenSource?.Cancel();
    }
}
