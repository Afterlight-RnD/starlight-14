// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT
using Content.Client._Starlight.Medical.Cybernetics.Systems;
using Content.Client._Starlight.Preferences.Systems;
using Content.Client.Humanoid;
using Content.Client.Lobby;
using Content.Shared._Starlight.CharacterProfileSystem;
using Content.Shared._Starlight.CharacterProfileSystem.Components;
using Content.Shared._Starlight.CharacterProfileSystem.Systems;
using Content.Shared.Clothing;
using Content.Shared.Humanoid;
using Content.Shared.Preferences;
using Robust.Client.GameObjects;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.UIEvents;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Client._Starlight.CharacterProfiles.Systems;

public sealed class CharacterProfileSystem : SharedCharacterProfileSystem, IUIEventSubscriber
{
    [Dependency] private readonly IUserInterfaceManager _uiManager = default!;
    [Dependency] private readonly IClientPreferencesManager _preferences = default!;
    [Dependency] private readonly HumanoidAppearanceSystem _humanoidSystem = default!;
    [Dependency] private readonly CyberneticsSystem _cyberSystem = default!;
    [Dependency] private readonly LoadoutSystem _loadoutSystem = default!;
    [Dependency] private readonly IPrototypeManager _protoManager = default!;
    [Dependency] private readonly PlayerPreferencesSystem _preferencesSystem = default!;
    public bool SlotsFull
    {
        get
        {
            //if pref data is not loaded, we can't check slots, so we assume full
            if (_preferences.Preferences == null || _preferences.Settings == null)
                return true;
            return _preferences.Preferences.Characters.Count >= _preferences.Settings.MaxCharacterSlots;
        }
    }

    public Entity<CharacterProfileComponent>? GetCharacterInSlot(int slot)
    {
        var preferencesEnt = _preferencesSystem.EnsurePlayerPreferences();
        if (!preferencesEnt.Comp.CharacterProfiles.TryGetValue(slot, out var profileEntity))
            return null;
        return (profileEntity,
            ProfileQuery.Comp(profileEntity));
    }

    public Entity<CharacterProfileComponent> GetFirstCharacterProfile()
    {
        var preferencesEnt = _preferencesSystem.EnsurePlayerPreferences();
        for (var i = 0; i < MaxProfileSlots; i++)
        {
            if (preferencesEnt.Comp.CharacterProfiles.TryGetValue(i, out var profileEnt))
                return (profileEnt, Comp<CharacterProfileComponent>(profileEnt));
        }
        throw new InvalidOperationException("No profiles found! At least one should be present!");
    }

    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<CharacterProfileComponent, AfterAutoHandleStateEvent>(OnReceivedUpdatedCharacter);
        SubscribeLocalEvent<CharacterProfileComponent, ComponentShutdown>(OnProfileShutdown);
    }

    private void OnProfileShutdown(Entity<CharacterProfileComponent> ent, ref ComponentShutdown args)
    {
        _uiManager.RaiseUIEvent(new CharacterProfileRemovedUIEvent(ent));
        EntityManager.DeleteEntity(ent.Comp.Doll);
    }

    private void OnReceivedUpdatedCharacter(Entity<CharacterProfileComponent> ent, ref AfterAutoHandleStateEvent ev)
    {
        if (ent.Comp.Doll == null)
        {
            ent.Comp.Doll = EntityManager.Spawn(_protoManager.Index(ent.Comp.Data.Profile.Species).DollPrototype,
                MapCoordinates.Nullspace);
            _uiManager.RaiseUIEvent( new CharacterProfileAddedUIEvent(ent,
                (ent.Comp.Doll.Value,
                    Comp<SpriteComponent>(ent.Comp.Doll.Value),
                    Comp<HumanoidAppearanceComponent>(ent.Comp.Doll.Value))));
        }
        else
            _uiManager.RaiseUIEvent(new CharacterProfileUpdatedEvent(ent));
    }

    public bool TryGetCharacterInSlot(int slot,
        out Entity<CharacterProfileComponent> profileEnt,
        out Entity<SpriteComponent, HumanoidAppearanceComponent> previewEntity)
    {
        var profile = GetCharacterInSlot(slot);
        if (profile?.Comp.Doll == null)
        {
            profileEnt = default;
            previewEntity = default;
            return false;
        }
        profileEnt = profile.Value;
        previewEntity =
            (profile.Value.Comp.Doll.Value,
            Comp<SpriteComponent>(profile.Value.Comp.Doll.Value),
            Comp<HumanoidAppearanceComponent>(profile.Value.Comp.Doll.Value));
        return true;
    }

    public void DirtyCharacter(Entity<CharacterProfileComponent> ent)
    {
        _uiManager.RaiseUIEvent(new CharacterProfileUpdatedUIEvent
        {
            CharacterProfile = new(ent.Owner, ent.Comp!)
        });
    }

    public void CreateNewCharacter()
    {
        //Legacy profile stuff
        _preferences.CreateCharacter(HumanoidCharacterProfile.Random());
    }

    public void SaveCharacterChanges(Entity<CharacterProfileComponent> target)
    {
        RaiseNetworkEvent(new MsgRequestCharacterProfileDataUpdate(
            GetNetEntity(target),
            GetOwningPlayerPrefsNet(target),
            target.Comp.Data, target.Comp.Slot));
    }

    private void UpdatePreviewEntity(Entity<SpriteComponent, HumanoidAppearanceComponent> previewEntity, Entity<CharacterProfileComponent> profileEntity)
    {
        _humanoidSystem.LoadProfile(previewEntity, profileEntity.Comp.Data.Profile, previewEntity.Comp2);
        _cyberSystem.ApplyCyberneticVisuals((previewEntity, previewEntity.Comp2), profileEntity.Comp.Data.Profile);
        _loadoutSystem.ApplyJobClothes(previewEntity, profileEntity);
    }
}