using Content.Client.Movement.Systems;
using Content.Shared.Aliens;
using Content.Shared.Devour;
using Content.Shared.Ghost;
using Content.Shared.Mobs;
using Content.Shared.Mobs.Components;

namespace Content.Client.Aliens;

/// <summary>
/// This handles...
/// </summary>
public sealed class AlienSystem : EntitySystem
{
    [Dependency] private readonly ContentEyeSystem _contentEye = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<EyeComponent, ToggleLightingAlienActionEvent>(OnToggleLighting);
        SubscribeLocalEvent<AlienComponent, DevourActionEvent>(OnDevour);
    }

    private void OnDevour(EntityUid uid, AlienComponent component, DevourActionEvent args)
    {

    }

    private void OnToggleLighting(EntityUid uid, EyeComponent component, ToggleLightingAlienActionEvent args)
    {
        if (args.Handled)
            return;

        RequestToggleLight(uid, component);
        args.Handled = true;
    }

    private void RequestToggleLight(EntityUid uid, EyeComponent? eye = null)
    {
        if (Resolve(uid, ref eye, false))
            _contentEye.RequestEye(eye.DrawFov, !eye.DrawLight);
    }
}
