using Robust.Shared.Audio;
using Robust.Shared.Prototypes;
using Content.Shared.Roles;
using Robust.Shared.Player;
using Content.Shared.Preferences;

namespace Content.Server.GameTicking.Rules.Components;

/// <summary>
/// Stores data for <see cref="ChangelingRuleSystem/">.
/// </summary>
[RegisterComponent, Access(typeof(ChangelingRuleSystem))]
public sealed partial class ChangelingRuleComponent : Component
{

    /// <summary>
    /// A chance for this mode to be added to the game.
    /// </summary>
    [DataField]
    public float RuleChance = 1f;

    [DataField]
    public ProtoId<AntagPrototype> ChangelingPrototypeId = "changeling";

    public Dictionary<ICommonSession, HumanoidCharacterProfile> StartCandidates = new();

    [DataField]
    public float MaxObjectiveDifficulty = 2.5f;

    [DataField]
    public int MaxObjectives = 10;

    /// <summary>
    /// All Thieves created by this rule
    /// </summary>
    [DataField]
    public List<EntityUid> ChangelingsMinds = new();

    /// <summary>
    /// Max Changelings created by rule on roundstart
    /// </summary>
    [DataField]
    public int MaxAllowChangeling = 3;

    /// <summary>
    /// Sound played when making the player a changeling via antag control or ghost role
    /// </summary>
    [DataField]
    public SoundSpecifier? GreetingSound = new SoundPathSpecifier("/Audio/Ambience/Antag/changeling_start.ogg");
}
