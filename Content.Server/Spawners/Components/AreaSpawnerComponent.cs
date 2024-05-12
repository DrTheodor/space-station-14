using System.Threading;
using Robust.Shared.Prototypes;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Server.Spawners.Components;

/// <summary>
/// Spawns Entities in area around spawner
/// </summary>
[RegisterComponent]
public sealed partial class AreaSpawnerComponent : Component
{
    // Maximum offset of entities spawned.
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField]
    public float Radius;

    // Prototype of entity spawned
    [ViewVariables(VVAccess.ReadWrite)]
    [DataField("spawnPrototype", customTypeSerializer: typeof(PrototypeIdSerializer<EntityPrototype>))]
    public string SpawnPrototype;

    /// <summary>
    /// Length of the interval between spawn attempts.
    /// </summary>
    [DataField]
    public int IntervalSeconds = 1;

    [ViewVariables]
    public int SpawnRadius;

    public CancellationTokenSource? TokenSource;
}
