using Content.Shared.Humanoid;

namespace Content.Shared.Changeling.Components;

/// <summary>
/// This is used for...
/// </summary>
[RegisterComponent]
public sealed partial class DnaBankEntryComponent : Component
{
    [ViewVariables(VVAccess.ReadWrite)]
    public HumanoidAppearanceComponent Appearance;

    [ViewVariables(VVAccess.ReadWrite)]
    public string Dna;

    [ViewVariables(VVAccess.ReadWrite)]
    public string Fingerprint;
}
