using System.Linq;
using Content.Server.Chat.Managers;
using Content.Server.GameTicking;
using Content.Server.Preferences.Managers;
using Content.Server.Station.Systems;
using Content.Shared.Preferences;
using Content.Shared.Roles;
using Robust.Shared.Player;
using Robust.Shared.Prototypes;
using Robust.Shared.Random;

namespace Content.Server.Jobs;

public sealed class RacistJobSystem : EntitySystem
{
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly IServerPreferencesManager _preferences = default!;
    [Dependency] private readonly IPrototypeManager _prototype = default!;
    [Dependency] private readonly IChatManager _chat = default!;

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<PlayerBeforeSpawnEvent>(OnBeforeSpawn);
    }

    public void OnBeforeSpawn(PlayerBeforeSpawnEvent ev)
    {
        if (ev.JobId == null)
            return;

        if (!_prototype.TryIndex<JobPrototype>(ev.JobId, out var job))
            return;

        var character = FindSuitableProfile(ev.Player, job) ?? CreateProfile(job);

        // Womp womp. Fluent doesn't support lists and their formatting! https://github.com/projectfluent/fluent/issues/79
        var allowedSpeciesText = string.Join(", ", job.AllowedSpecies);
        _chat.DispatchServerMessage(ev.Player, Loc.GetString("player-racist", ("allowed", allowedSpeciesText)));

        ev.Profile = character;
    }

    public HumanoidCharacterProfile CreateProfile(JobPrototype job)
    {
        return HumanoidCharacterProfile.RandomWithSpecies(_random.Pick(job.AllowedSpecies));
    }

    public HumanoidCharacterProfile? FindSuitableProfile(ICommonSession player, JobPrototype job)
    {
        foreach (var preference in GetPlayerPreferences(player).Characters)
        {
            var character = (HumanoidCharacterProfile) preference.Value;

            if (AllowedOnJob(character, job))
                return character;
        }

        return null;
    }

    public PlayerPreferences GetPlayerPreferences(ICommonSession p)
    {
        return _preferences.GetPreferences(p.UserId);
    }

    public bool AllowedOnJob(HumanoidCharacterProfile profile, JobPrototype job)
    {
        return AllowedOnJob(profile.Species, job);
    }

    public bool AllowedOnJob(string species, JobPrototype job)
    {
        return job.AllowedSpecies.Count == 0 || job.AllowedSpecies.Contains(species);
    }
}
