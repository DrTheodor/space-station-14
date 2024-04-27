using Content.Client.Movement.Systems;
using ToggleLightingAlienActionEvent = Content.Shared.Aliens.Components.ToggleLightingAlienActionEvent;

namespace Content.Client.Aliens.Systems;

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
