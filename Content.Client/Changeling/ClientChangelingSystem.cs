using Content.Client.Store.Ui;
using Content.Shared.Changeling.Components;
using Content.Shared.Store;

namespace Content.Client.Changeling;

/// <summary>
/// This handles...
/// </summary>
public sealed class ClientChangelingSystem : EntitySystem
{
    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<ChangelingComponent, MapInitEvent>(OnInit);
    }

    private void OnInit(EntityUid uid, ChangelingComponent component, MapInitEvent args)
    {

    }
}
