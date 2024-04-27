using Content.Shared.Buckle.Components;
using Content.Shared.Cuffs;
using Content.Shared.Cuffs.Components;
using Robust.Shared.Spawners;

namespace Content.Shared.Buckle;

/// <summary>
/// This handles...
/// </summary>
public sealed class CuffOnBuckleSystem : EntitySystem
{
    [Dependency] private readonly SharedCuffableSystem _cuffable = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<CuffOnBuckleComponent, BuckleChangeEvent>(OnBuckleChange);
    }

    private void OnBuckleChange(EntityUid uid, CuffOnBuckleComponent component, BuckleChangeEvent args)
    {
        if (!args.Buckling)
        {
            return;
        }
        var cuffsEntity = Spawn(component.CuffsPrototype, Transform(args.StrapEntity).Coordinates);
        if (!_cuffable.TryCuffing(args.StrapEntity, uid, cuffsEntity, null, null, false))
        {
            QueueDel(cuffsEntity);
        }
    }
}
