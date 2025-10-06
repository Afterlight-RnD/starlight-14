// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Server._Starlight.CharacterProfiles.Systems;
using Content.Shared._Starlight.CharacterProfileSystem.Components;
using Content.Shared._Starlight.Preferences.Components;
using Content.Shared._Starlight.Preferences.Systems;
using Content.Shared.Preferences;
using Robust.Server.GameStates;
using Robust.Shared.Map;
using Robust.Shared.Network;
using Robust.Shared.Player;

namespace Content.Server.Preferences.Managers.Systems;

/// <summary>
/// This handles...
/// </summary>
public sealed class PlayerPreferenceSystem : SharedPlayerPreferenceSystem
{
    [Dependency] private readonly PvsOverrideSystem _pvsOverride = default!;
    [Dependency] private readonly CharacterProfileSystem _characterProfileSystem = default!;

    private Dictionary<NetUserId, Entity<PlayerPreferencesComponent>> _preferenceLookup = new();

    public override void Initialize()
    {
        SubscribeLocalEvent<LegacyPlayerPreferencesLoadStartedEvent>(OnLegacyPrefsLoadStart);
        SubscribeLocalEvent<LegacyPlayerPreferencesLoadedEvent>(OnLegacyPrefsLoaded);
        SubscribeLocalEvent<LegacyPlayerPreferencesUnloadedEvent>(OnLegacyPrefsUnloaded);
    }

    private void OnLegacyPrefsLoadStart(LegacyPlayerPreferencesLoadStartedEvent ev)
    {
        CreatePreferencesEntity(ev.Session);
    }

    private void OnLegacyPrefsUnloaded(LegacyPlayerPreferencesUnloadedEvent ev)
    {
        ClearCachedPreferences(ev.Session);
    }

    private void OnLegacyPrefsLoaded(LegacyPlayerPreferencesLoadedEvent ev)
    {
        FinishPreferenceLoad(ev.Session, ev.Preferences);
    }


    public override Entity<PlayerPreferencesComponent>? GetPlayerPreferences(NetUserId userId)
    {
        if (_preferenceLookup.TryGetValue(userId, out var foundPref) && foundPref.Comp.Loaded)
            return foundPref;
        Log.Warning($"PlayerPref for User:{userId} could not be found or is not yet loaded!");
        return null;
    }

    private Entity<PlayerPreferencesComponent> CreatePreferencesEntity(ICommonSession playerSession)
    {
        if (_preferenceLookup.TryGetValue(playerSession.UserId, out var existing))
        {
            Log.Error($"User:{playerSession.UserId} already has loaded preferences!");
            return existing;
        }
        var newEnt = EntityManager.Spawn(null, MapCoordinates.Nullspace);
        var prefComp = AddComp<PlayerPreferencesComponent>(newEnt);
        var newPrefs = new Entity<PlayerPreferencesComponent>(newEnt, prefComp);
        newPrefs.Comp.OwnerNetId = playerSession.UserId;

        _pvsOverride.AddSessionOverride(newEnt,playerSession);
        _preferenceLookup.Add(playerSession.UserId, newPrefs);
        return newPrefs;
    }

    public void FinishPreferenceLoad(ICommonSession playerSession, PlayerPreferences? preferences)
    {
        if (!_preferenceLookup.TryGetValue(playerSession.UserId, out var prefs))
        {
            Log.Error($"User:{playerSession.UserId} does not have an associated preference entity!");
            return;
        }
        if (preferences != null)
            ConvertLegacyPrefs(playerSession, prefs, preferences);
        prefs.Comp.Loaded = true;

        Dirty(prefs);
        RaiseLocalEvent(new PlayerPreferencesLoadedEvent(playerSession, prefs));
    }

    public void ClearCachedPreferences(ICommonSession playerSession)
    {
        if (_preferenceLookup.Remove(playerSession.UserId, out var prefs))
            EntityManager.Deleted(prefs); //This should delete all character profiles as well since those are parented
    }

    private void ConvertLegacyPrefs(ICommonSession session,Entity<PlayerPreferencesComponent> prefs, PlayerPreferences legacyPrefs)
    {
        ConvertJobs(prefs, legacyPrefs);
        ConvertCharacters(session, prefs, legacyPrefs);
    }

    private void ConvertJobs(Entity<PlayerPreferencesComponent> prefs, PlayerPreferences legacyPrefs)
    {
        if (legacyPrefs.JobPriorities.Count == 0)
            return;
        foreach (var (jobId, priority) in legacyPrefs.JobPriorities)
        {
            switch (priority)
            {
                case JobPriority.Low:
                    prefs.Comp.JobPreferences.Low ??= new();
                    prefs.Comp.JobPreferences.Low.Add(jobId);
                    break;
                case JobPriority.Medium:
                    prefs.Comp.JobPreferences.Medium ??= new();
                    prefs.Comp.JobPreferences.Medium.Add(jobId);
                    break;
                case JobPriority.High:
                    prefs.Comp.JobPreferences.High = jobId;
                    break;
                case JobPriority.Never:
                    {
                        prefs.Comp.JobPreferences.Never ??= new();
                        prefs.Comp.JobPreferences.Never.Add(jobId);
                        break;
                    }
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    private void ConvertCharacters(ICommonSession session, Entity<PlayerPreferencesComponent> prefs, PlayerPreferences legacyPrefs)
    {
        foreach (var (slot, profile) in legacyPrefs.Characters)
        {
            if (profile is not HumanoidCharacterProfile humanoid)
            {
                Log.Fatal("Only humanoid profiles are supported for characters!");
                return;
            }
            _characterProfileSystem.LoadProfile(session, prefs, slot, new CharacterProfileData(humanoid));
        }
    }
}