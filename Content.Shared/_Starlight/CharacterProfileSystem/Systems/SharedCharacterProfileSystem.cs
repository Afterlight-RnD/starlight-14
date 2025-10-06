// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.CharacterProfileSystem.Components;
using Content.Shared._Starlight.Preferences.Components;
using Content.Shared.CCVar;
using Content.Shared.Roles;
using Content.Shared.Starlight.CCVar;
using Robust.Shared.Configuration;
using Robust.Shared.Prototypes;

namespace Content.Shared._Starlight.CharacterProfileSystem.Systems;

/// <summary>
/// Handles shared character profile data
/// </summary>
public abstract class SharedCharacterProfileSystem : EntitySystem
{
    [Dependency] protected readonly IConfigurationManager Cfg = default!;
    [Dependency] protected readonly SharedTransformSystem TransformSystem = default!;

    protected EntityQuery<CharacterProfileComponent> ProfileQuery;

    protected EntityQuery<PlayerPreferencesComponent> PlayerPrefQuery;

    protected int MaxProfileSlots = -1;

    private ProtoId<JobPrototype> _fallbackJob;

    public override void Initialize()
    {
        Cfg.OnValueChanged(CCVars.GameMaxCharacterSlots, OnMaxProfileSlotsChanged, true);
        Cfg.OnValueChanged(StarlightCCVars.FallbackJob, OnFallbackJobChanged, true);
    }

    public NetEntity GetOwningPlayerPrefsNet(Entity<CharacterProfileComponent> profile)
    {
        var prefEnt = TransformSystem.GetParentUid(profile);
        return GetNetEntity(prefEnt);
    }

    public Entity<PlayerPreferencesComponent> GetOwningPlayerPrefs(Entity<CharacterProfileComponent> profile)
    {
        var prefEnt = TransformSystem.GetParentUid(profile);
        return (prefEnt, Comp<PlayerPreferencesComponent>(prefEnt));
    }

    private void OnFallbackJobChanged(string job)
    {
        _fallbackJob = new ProtoId<JobPrototype>(job);
    }

    // private void UpdatePreviewJob(ref JobPrioritiesUpdatedEvent ev)
    // {
    //     while (EntityQueryEnumerator<CharacterProfileComponent>().MoveNext(out var ent, out var profile))
    //     {
    //         if (ev.HighPriority != null && profile.EnabledJobs.Contains(ev.HighPriority.Value))
    //         {
    //             if (ev.HighPriority.Value == profile.PreviewJob)
    //                 return;
    //             profile.PreviewJob = ev.HighPriority.Value;
    //             Dirty(ent, profile);
    //             return;
    //         }
    //
    //         foreach (var job in profile.EnabledJobs)
    //         {
    //             if (ev.MediumPriority != null)
    //             {
    //                 if (ev.MediumPriority.Contains(job))
    //                 {
    //                     if (profile.PreviewJob == job)
    //                         return;
    //                     profile.PreviewJob = job;
    //                     Dirty(ent, profile);
    //                     return;
    //                 }
    //             }
    //             if (ev.LowPriority != null)
    //             {
    //                 if (ev.LowPriority.Contains(job))
    //                 {
    //                     if (profile.PreviewJob == job)
    //                         return;
    //                     profile.PreviewJob = job;
    //                     Dirty(ent, profile);
    //                     return;
    //                 }
    //             }
    //         }
    //         if (profile.PreviewJob == _fallbackJob)
    //             return;
    //         profile.PreviewJob = _fallbackJob;
    //         Dirty(ent, profile);
    //     }
    // }

    protected virtual void OnMaxProfileSlotsChanged(int newMax)
    {
        MaxProfileSlots = newMax;
    }
}