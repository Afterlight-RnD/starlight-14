// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Linq;
using Content.Client._Starlight.CharacterProfiles.Systems;
using Content.Client._Starlight.UI.Systems.CharacterView.Controls;
using Content.Client.Humanoid;
using Content.Shared._Starlight.CharacterProfileSystem.Components;
using Content.Shared.Body.Part;
using Content.Shared.Humanoid;
using Content.Shared.Preferences;
using Content.Shared.Starlight;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.UIEvents;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Client._Starlight.UI.Systems.CharacterView;

/// <summary>
/// </summary>
public sealed class HumanoidViewSystem : EntitySystem, IUIEventSubscriber
{
    [Dependency] private readonly IUserInterfaceManager _uiMan = default!;
    [Dependency] private readonly CharacterProfileSystem _profileSystem = default!;
    [Dependency] private readonly HumanoidAppearanceSystem _humanoidSystem = default!;
    [Dependency] private readonly IPrototypeManager _protoMan = default!;
    public override void Initialize()
    {
        _uiMan.SubscribeUIEvent<CharacterSlotView, RefreshProfilePreviewUIEvent>(this, RefreshCharacterView);
    }

    private void RefreshCharacterView(CharacterSlotView control, RefreshProfilePreviewUIEvent ev)
    {
        control.SetEntity(null);
        if (control.Slot >= 0 &&
            _profileSystem.TryGetCharacterInSlot(control.Slot, out var profileEnt)
            && profileEnt.Comp.Data.Profile != null)
        {
            var viewEnt = EnsureViewEntity(profileEnt);
            //TODO: Might need to clear appearance first?
            _humanoidSystem.LoadProfile(viewEnt, profileEnt.Comp.Data.Profile, viewEnt.Comp);
            ApplyCybernetics(viewEnt, profileEnt.Comp.Data.Profile);
            control.SetEntity(viewEnt);
        }
    }

    public void SetCharacterView(SpriteView target, CharacterProfileData? profileData)
    {
        if (target is { Entity: not null, EntityIsTransient: false })
            return;

        if (profileData == null || profileData.Profile == null)
        {
            target.SetEntity(null);
            return;
        }

        if (TryComp(target.Entity, out HumanoidAppearanceComponent? appearanceComponent))
        {
            //TODO: May need to clear old appearance data?
            _humanoidSystem.LoadProfile(target.Entity.Value, profileData.Profile, appearanceComponent);
            ApplyCybernetics((target.Entity.Value, appearanceComponent), profileData.Profile);
            return;
        }

        if (profileData.Profile == null)
            return;

        var newEnt = Spawn(_protoMan.Index(profileData.Profile.Species).DollPrototype, MapCoordinates.Nullspace);
        _humanoidSystem.LoadProfile(newEnt ,profileData.Profile, appearanceComponent);
        ApplyCybernetics((newEnt, Comp<HumanoidAppearanceComponent>(newEnt)), profileData.Profile);
        target.SetEntity(newEnt, true);
    }

    //TODO: this is a stopgap until humanoidCharacter refactor cleans up cybernetics code
    private void ApplyCybernetics(Entity<HumanoidAppearanceComponent> humanoidAppearance, HumanoidCharacterProfile humanoid)
    {
        Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo> cyberLayers = humanoid.Cybernetics.Select(p =>
        {
            var _cyberneticEnt = _protoMan.Index<EntityPrototype>(p);
            if (_cyberneticEnt.TryGetComponent<BodyPartComponent>(out var part, EntityManager.ComponentFactory) &&
                _cyberneticEnt.TryGetComponent<BaseLayerIdComponent>(out var layer, EntityManager.ComponentFactory))
            {
                return (CyberneticImplant.LayerFromBodypart(part), new(layer.Layer));
            }
            else { return (HumanoidVisualLayers.Special, new CustomBaseLayerInfo()); }
        }).Where(p => p.Item1 != HumanoidVisualLayers.Special).ToDictionary();
        _humanoidSystem.AddCustomBaseLayers(humanoidAppearance, cyberLayers);
    }
    public Entity<HumanoidAppearanceComponent> EnsureViewEntity(Entity<CharacterProfileComponent> profileEnt)
    {
        if (profileEnt.Comp.PreviewEntity != null)
            return profileEnt.Comp.PreviewEntity.Value;
        //Profile is already checked before this gets called so we suppress the nullable
        var newEnt = EntityManager.SpawnEntity(_protoMan.Index(profileEnt.Comp.Data.Profile!.Species).DollPrototype,
            MapCoordinates.Nullspace);
        Entity<HumanoidAppearanceComponent> newView = (newEnt, Comp<HumanoidAppearanceComponent>(newEnt));
        profileEnt.Comp.PreviewEntity = newView;
        return newView;
    }
}