// SPDX-FileCopyrightText: 2025 Afterlight RnD
// SPDX-License-Identifier: ASL-1.0

using System.Linq;
using Content.Client._Starlight.Character.UIView.Components;
using Content.Client.Humanoid;
using Content.Shared.Body.Part;
using Content.Shared.Humanoid;
using Content.Shared.Preferences;
using Content.Shared.Starlight;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Client._Starlight.Character.UIView.Systems;

/// <summary>
/// Clientside system for visualizing characters in UI controls
/// </summary>
public sealed class CharacterUIViewSystem : EntitySystem
{
    [Dependency] private readonly IPrototypeManager _protoManager = default!;
    [Dependency] private readonly HumanoidAppearanceSystem _appearanceSystem = default!;

    private EntityQuery<CharacterUIViewComponent> _viewQuery;
    private EntityQuery<HumanoidAppearanceComponent> _humanoidQuery;

    public override void Initialize()
    {
        _viewQuery = GetEntityQuery<CharacterUIViewComponent>();
        _humanoidQuery = GetEntityQuery<HumanoidAppearanceComponent>();
    }

    public bool RemoveView(Entity<CharacterUIViewComponent?> viewEntity)
    {
        if (!_viewQuery.Resolve(ref viewEntity, false))
            return true;
        EntityManager.DeleteEntity(viewEntity);
        return true;
    }

    public bool UpdateBaseView(Entity<CharacterUIViewComponent?, HumanoidAppearanceComponent?> viewEntity,
        HumanoidCharacterProfile characterProfile)
    {
        if (!_viewQuery.Resolve(viewEntity.Owner, ref viewEntity.Comp1)
            || !_humanoidQuery.Resolve(viewEntity.Owner, ref viewEntity.Comp2))
            return false;
        _appearanceSystem.LoadProfile(viewEntity, characterProfile, viewEntity.Comp2);
        var layers = GetCyberneticsLayers(characterProfile);
        _appearanceSystem.AddCustomBaseLayers(viewEntity, layers);
        return true;
    }

    public Entity<CharacterUIViewComponent, HumanoidAppearanceComponent> CreateBaseView(
        HumanoidCharacterProfile characterProfile)
    {
        var previewEnt = Spawn(_protoManager.Index(characterProfile.Species).DollPrototype,
            MapCoordinates.Nullspace);
        var appearanceComp = Comp<HumanoidAppearanceComponent>(previewEnt);
        var previewComp = AddComp<CharacterUIViewComponent>(previewEnt);
        _appearanceSystem.LoadProfile(previewEnt, characterProfile, appearanceComp);
        var layers = GetCyberneticsLayers(characterProfile);
        _appearanceSystem.AddCustomBaseLayers(previewEnt, layers);
        return new(previewEnt, previewComp, appearanceComp);
    }

    /// Starlight
    /// <summary>
    /// Extracts cybernetics IDs from humanoid profile, returns all their visual layers
    /// </summary>
    private Dictionary<HumanoidVisualLayers, CustomBaseLayerInfo> GetCyberneticsLayers(
        HumanoidCharacterProfile humanoid)
    {
        return humanoid.Cybernetics.Select(p =>
        {
            var _cyberneticEnt = _protoManager.Index<EntityPrototype>(p);
            if (_cyberneticEnt.TryGetComponent<BodyPartComponent>(out var part, EntityManager.ComponentFactory) &&
                _cyberneticEnt.TryGetComponent<BaseLayerIdComponent>(out var layer, EntityManager.ComponentFactory))
            {
                return (CyberneticImplant.LayerFromBodypart(part), new(layer.Layer));
            }
            else { return (HumanoidVisualLayers.Special, new CustomBaseLayerInfo()); }
        }).Where(p => p.Item1 != HumanoidVisualLayers.Special).ToDictionary();
    }
}