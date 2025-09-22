// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Linq;
using Content.Client.Humanoid;
using Content.Shared._Starlight.Medical.Cybernetics.Systems;
using Content.Shared.Body.Part;
using Content.Shared.Humanoid;
using Content.Shared.Preferences;
using Content.Shared.Starlight;
using EntityPrototype = Robust.Shared.Prototypes.EntityPrototype;

namespace Content.Client._Starlight.Medical.Cybernetics.Systems;

public sealed class CyberneticsSystem : SharedCyberneticsSystem
{
    [Dependency] private readonly HumanoidAppearanceSystem _humanoidSystem = default!;


    public override void ApplyCyberneticVisuals(Entity<HumanoidAppearanceComponent> humanoidAppearance,
        HumanoidCharacterProfile? humanoid)
    {
        if (humanoid == null)
            return;
        var cyberLayers = humanoid.Cybernetics.Select(p =>
        {
            var cyberneticEnt = ProtoMan.Index<EntityPrototype>(p);
            if (cyberneticEnt.TryGetComponent<BodyPartComponent>(out var part, EntityManager.ComponentFactory) &&
                cyberneticEnt.TryGetComponent<BaseLayerIdComponent>(out var layer, EntityManager.ComponentFactory))
            {
                return (CyberneticImplant.LayerFromBodypart(part), new(layer.Layer));
            }
            else { return (HumanoidVisualLayers.Special, new CustomBaseLayerInfo()); }
        }).Where(p => p.Item1 != HumanoidVisualLayers.Special).ToDictionary();
        _humanoidSystem.AddCustomBaseLayers(humanoidAppearance, cyberLayers);
    }
}