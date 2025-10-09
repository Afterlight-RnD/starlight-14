// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.CharacterProfileSystem.Components;
using Content.Shared.Inventory;
using Content.Shared.Preferences.Loadouts;
using Content.Shared.Roles;
using Robust.Shared.Map;
using Robust.Shared.Prototypes;

namespace Content.Shared.Clothing;

/// <summary>
/// Assigns a loadout to an entity based on the RoleLoadout prototype
/// </summary>
public sealed partial class LoadoutSystem : EntitySystem
{
    [Dependency] private readonly InventorySystem _inventorySystem = default!;

    public RoleLoadout CreateDefaultLoadout(ProtoId<JobPrototype> jobProto)
    {
        var roleProto = new RoleLoadout(GetJobPrototype(jobProto));
        var roleLoadoutProto = _protoMan.Index(roleProto.Role);
        //I swear I got an anhuerism trying to understand this, redo all of this eventually
        for (var i = roleLoadoutProto.Groups.Count - 1; i >= 0; i--)
        {
            var group = roleLoadoutProto.Groups[i];

            if (!_protoMan.TryIndex(group, out var groupProto))
                continue;

            if (roleProto.SelectedLoadouts.ContainsKey(group))
                continue;

            var loadouts = new List<Loadout>();
            roleProto.SelectedLoadouts[group] = loadouts;

            if (groupProto.MinLimit > 0)
            {
                // Apply any loadouts we can.
                foreach (var protoId in groupProto.Loadouts)
                {
                    // Reached the limit, time to stop
                    if (loadouts.Count >= groupProto.MinLimit)
                        break;

                    if (!_protoMan.TryIndex(protoId, out var loadoutProto))
                        continue;

                    var defaultLoadout = new Loadout()
                    {
                        Prototype = loadoutProto.ID,
                    };

                    loadouts.Add(defaultLoadout);
                    foreach (var effect in loadoutProto.Effects)
                    {
                        effect.Apply(roleProto);
                    }
                }
            }
        }
        return roleProto;
    }

    public void ApplyJobClothes(EntityUid target, Entity<CharacterProfileComponent> profile,
        ProtoId<JobPrototype>? jobOverride = null)
    {
        if (!_inventorySystem.TryGetSlots(target, out var slots))
            return;

        var jobProto = jobOverride ?? profile.Comp.PreviewJob;
        var job = _protoMan.Index(jobProto);
        if (!profile.Comp.JobLoadouts.TryGetValue(jobProto, out var data))
        {
            data = CreateDefaultLoadout(jobProto);
        }
        foreach (var loadouts in data.SelectedLoadouts.Values)
        {
            foreach (var loadout in loadouts)
            {
                if (!_protoMan.TryIndex(loadout.Prototype, out var loadoutProto))
                    continue;

                // TODO: Need some way to apply starting gear to an entity and replace existing stuff coz holy fucking shit dude.
                foreach (var slot in slots)
                {
                    // Try startinggear first
                    if (_protoMan.TryIndex(loadoutProto.StartingGear, out var loadoutGear))
                    {
                        var itemType = ((IEquipmentLoadout)loadoutGear).GetGear(slot.Name);
                        if (itemType != string.Empty)
                        {
                            if (_inventorySystem.TryUnequip(target, slot.Name, out var unequippedItem, silent: true,
                                    force: true, reparent: false))
                            {
                                EntityManager.DeleteEntity(unequippedItem.Value);
                            }

                            var item = EntityManager.SpawnEntity(itemType, MapCoordinates.Nullspace);
                            _inventorySystem.TryEquip(target, item, slot.Name, true, true);
                        }
                    }
                    else
                    {
                        var itemType = ((IEquipmentLoadout)loadoutProto).GetGear(slot.Name);
                        if (itemType != string.Empty)
                        {
                            if (_inventorySystem.TryUnequip(target, slot.Name, out var unequippedItem, silent: true,
                                    force: true, reparent: false))
                            {
                                EntityManager.DeleteEntity(unequippedItem.Value);
                            }

                            var item = EntityManager.SpawnEntity(itemType, MapCoordinates.Nullspace);
                            _inventorySystem.TryEquip(target, item, slot.Name, true, true);
                        }
                    }
                }
            }
        }

        if (!_protoMan.TryIndex(job.StartingGear, out var gear))
            return;

        foreach (var slot in slots)
        {
            var itemType = ((IEquipmentLoadout)gear).GetGear(slot.Name);
            if (itemType != string.Empty)
            {
                if (_inventorySystem.TryUnequip(target, slot.Name, out var unequippedItem, silent: true, force: true,
                        reparent: false))
                {
                    EntityManager.DeleteEntity(unequippedItem.Value);
                }

                var item = EntityManager.SpawnEntity(itemType, MapCoordinates.Nullspace);
                _inventorySystem.TryEquip(target, item, slot.Name, true, true);
            }
        }
    }
}