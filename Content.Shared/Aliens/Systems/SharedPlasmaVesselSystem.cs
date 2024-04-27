using Content.Shared.Alert;
using Content.Shared.FixedPoint;
using PlasmaVesselComponent = Content.Shared.Aliens.Components.PlasmaVesselComponent;

namespace Content.Shared.Aliens.Systems;

/// <summary>
/// This handles...
/// </summary>
public sealed class SharedPlasmaVesselSystem : EntitySystem
{
    /// <inheritdoc/>
    [Dependency] private readonly AlertsSystem _alerts = default!;
    public override void Initialize()
    {

    }

    public bool ChangePlasmaAmount(EntityUid uid, FixedPoint2 amount, PlasmaVesselComponent? component = null, bool regenCap = false)
    {
        if (!Resolve(uid, ref component))
            return false;

        component.Plasma += amount;

        if (regenCap)
            FixedPoint2.Min(component.Plasma, component.PlasmaRegenCap);

        // _alerts.ShowAlert(uid, AlertType.Essence, (short) Math.Clamp(Math.Round(component.Plasma.Float() / 10f), 0, 16));

        return true;
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<PlasmaVesselComponent>();
        while (query.MoveNext(out var uid, out var rev))
        {
            rev.Accumulator += frameTime;

            if (rev.Accumulator <= 1)
                continue;
            rev.Accumulator -= 1;

            if (rev.Plasma < rev.PlasmaRegenCap)
            {
                ChangePlasmaAmount(uid, rev.PlasmaPerSecond, rev, regenCap: true);
            }
        }
    }
}
