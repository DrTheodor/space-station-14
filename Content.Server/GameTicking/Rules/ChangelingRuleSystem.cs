using Content.Server.Chat.Managers;
using Content.Server.GameTicking.Rules.Components;
using Content.Server.Mind;
using Content.Server.Objectives;
using Content.Server.Roles;
using Content.Shared.Mind;
using Content.Shared.Objectives.Components;
using Content.Shared.Roles;
using Content.Shared.Roles.Jobs;
using Robust.Shared.Player;
using Robust.Shared.Random;
using Robust.Shared.Prototypes;
using System.Linq;
using Content.Shared.Humanoid;
using Content.Server.Antag;
using Content.Server.Store.Components;
using Content.Server.Store.Systems;
using Content.Shared.Changeling.Components;
using Robust.Server.Audio;
using Content.Shared.CombatMode.Pacification;
using Content.Shared.Random;

namespace Content.Server.GameTicking.Rules;

public sealed class ChangelingRuleSystem : GameRuleSystem<ChangelingRuleComponent>
{
    [Dependency] private readonly IChatManager _chatManager = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly AntagSelectionSystem _antagSelection = default!;
    [Dependency] private readonly AudioSystem _audio = default!;
    [Dependency] private readonly MindSystem _mindSystem = default!;
    [Dependency] private readonly SharedRoleSystem _roleSystem = default!;
    [Dependency] private readonly ObjectivesSystem _objectives = default!;
    [Dependency] private readonly StoreSystem _store = default!;

    [ValidatePrototypeId<WeightedRandomPrototype>]
    const string SmallObjectiveGroup = "TraitorObjectiveGroups";
    [ValidatePrototypeId<WeightedRandomPrototype>]
    const string KillObjectiveGroup = "KillObjectiveGroups";

    public override void Initialize()
    {
        base.Initialize();

        SubscribeLocalEvent<RulePlayerJobsAssignedEvent>(OnPlayersSpawned);

        SubscribeLocalEvent<ChangelingRoleComponent, GetBriefingEvent>(OnGetBriefing);
        SubscribeLocalEvent<ChangelingRuleComponent, ObjectivesTextGetInfoEvent>(OnObjectivesTextGetInfo);
    }

    private void OnPlayersSpawned(RulePlayerJobsAssignedEvent ev)
    {
        var query = EntityQueryEnumerator<ChangelingRuleComponent, GameRuleComponent>();
        while (query.MoveNext(out var uid, out var changeling, out var gameRule))
        {
            //Chance to not lauch gamerule
            if (_random.Prob(changeling.RuleChance))
            {
                if (!GameTicker.IsGameRuleAdded(uid, gameRule))
                    continue;

                foreach (var player in ev.Players)
                {
                    if (!ev.Profiles.TryGetValue(player.UserId, out var profile))
                        continue;

                    changeling.StartCandidates[player] = profile;
                }
                DoChangelingStart(changeling);
            }
        }
    }

    private void DoChangelingStart(ChangelingRuleComponent component)
    {
        if (!component.StartCandidates.Any())
        {
            Log.Error("There are no players who can become changelings.");
            return;
        }

        var startChangelingCount = Math.Min(component.MaxAllowChangeling, component.StartCandidates.Count);
        var changelingPool = _antagSelection.FindPotentialAntags(component.StartCandidates, component.ChangelingPrototypeId);
        //TO DO: When voxes specifies are added, increase their chance of becoming a changeling by 4 times >:)
        var selectedChangelings = _antagSelection.PickAntag(_random.Next(1, startChangelingCount), changelingPool);

        foreach(var changeling in selectedChangelings)
        {
            MakeChangeling(component, changeling);
        }
    }

    public bool MakeChangeling(ChangelingRuleComponent changelingRule, ICommonSession changeling)
    {
        //checks
        if (!_mindSystem.TryGetMind(changeling, out var mindId, out var mind))
        {
            Log.Info("Failed getting mind for picked changeling.");
            return false;
        }
        if (HasComp<ChangelingRoleComponent>(mindId))
        {
            Log.Error($"Player {changeling.Name} is already a changeling.");
            return false;
        }
        if (mind.OwnedEntity is not { } entity)
        {
            Log.Error("Mind picked for changeling did not have an attached entity.");
            return false;
        }

        // Assign changeling roles
        _roleSystem.MindAddRole(mindId, new ChangelingRoleComponent
        {
            PrototypeId = changelingRule.ChangelingPrototypeId
        });
        AddComp<ChangelingComponent>((EntityUid) mind.OwnedEntity);

        var store = EnsureComp<StoreComponent>((EntityUid) mind.OwnedEntity);
        _store.InitializeFromPreset("StorePresetEvolution", (EntityUid) mind.OwnedEntity, store);
        store.AccountOwner = (EntityUid) mind.OwnedEntity;

        // Notificate player about new role assignment
        if (_mindSystem.TryGetSession(mindId, out var session))
        {
            _audio.PlayGlobal(changelingRule.GreetingSound, session);
            _chatManager.DispatchServerMessage(session, Loc.GetString("changeling-role-greeting"));
        }

        // Give changelings their objectives
        var difficulty = 0f;

        for (var i = 0; i < changelingRule.MaxObjectives && changelingRule.MaxObjectiveDifficulty > difficulty; i++)  // Many small objectives
        {
            var objective = _objectives.GetRandomObjective(mindId, mind, SmallObjectiveGroup);
            if (objective == null)
                continue;

            _mindSystem.AddObjective(mindId, mind, objective.Value);
            difficulty += Comp<ObjectiveComponent>(objective.Value).Difficulty;
        }

        var escapeObjective = _objectives.GetRandomObjective(mindId, mind, KillObjectiveGroup);
        if (escapeObjective != null)
            _mindSystem.AddObjective(mindId, mind, escapeObjective.Value);

        changelingRule.ChangelingsMinds.Add(mindId);
        return true;
    }

    public void AdminMakeChangeling(ICommonSession changeling)
    {
        var changelingRule = EntityQuery<ChangelingRuleComponent>().FirstOrDefault();
        if (changelingRule == null)
        {
            GameTicker.StartGameRule("Changeling", out var ruleEntity);
            changelingRule = Comp<ChangelingRuleComponent>(ruleEntity);
        }

        MakeChangeling(changelingRule, changeling);
    }

    //Add mind briefing
    private void OnGetBriefing(Entity<ChangelingRoleComponent> changeling, ref GetBriefingEvent args)
    {
        if (!TryComp<MindComponent>(changeling.Owner, out var mind) || mind.OwnedEntity == null)
            return;
        args.Append(Loc.GetString("changeling-role-greeting"));
    }

    private void OnObjectivesTextGetInfo(Entity<ChangelingRuleComponent> changelings, ref ObjectivesTextGetInfoEvent args)
    {
        args.Minds = changelings.Comp.ChangelingsMinds;
        args.AgentName = Loc.GetString("changeling-round-end-agent-name");
    }
}
