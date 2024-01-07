using System.Collections;
using Content.Shared.Actions;
using Content.Shared.FixedPoint;
using Content.Shared.Humanoid;

namespace Content.Shared.Changeling.Components;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent]
public sealed partial class ChangelingComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite)]
    public FixedPoint2 Chemicals = 75;

    /// <summary>
    /// The entity's current max amount of chemicals.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField("maxChemicals")]
    public FixedPoint2 ChemicalsRegenCap = 75;

    /// <summary>
    /// The amount of chemicals passively generated per second.
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite), DataField("ChemicalsPerSecond")]
    public FixedPoint2 ChemicalsPerSecond = 1.0f;

    [ViewVariables(VVAccess.ReadWrite)]
    public Dictionary<string, HumanoidAppearanceComponent> DnaBank;

    [ViewVariables]
    public float Accumulator = 0;

    [DataField("actionEvolveEntity")]
    public EntityUid? ActionEvolveEntity;

    [DataField("actionTransformEntity")]
    public EntityUid? ActionTransformEntity;
}

public sealed partial class ChangelingEvolveActionEvent : InstantActionEvent
{
}

public sealed partial class ChangelingStingExtractActionEvent : EntityTargetActionEvent
{
}

public sealed partial class ChangelingTransformActionEvent : InstantActionEvent
{
}


