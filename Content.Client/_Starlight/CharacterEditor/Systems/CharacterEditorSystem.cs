// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.CharacterProfiles.Systems;
using Content.Client._Starlight.ProfileEditor;
using Content.Shared._Starlight.CharacterProfiles;
using Robust.Client.GameObjects;

namespace Content.Client._Starlight.CharacterEditor.Systems;

public sealed partial class CharacterEditorSystem : ProfileEditorSystem<CharacterEditorControl, CharacterEditorStep>
{
    [Dependency] private readonly CharacterProfileSystem _characterProfileSystem = default!;
    public Entity<SpriteComponent> EnsurePreviewEntity(CharacterEditorControl editorControl)
    {
        if (editorControl.LiveProfile == null)
             throw new InvalidOperationException("Cannot ensure preview without a live profile!");
        if (editorControl.PreviewEntity != null)
            return editorControl.PreviewEntity.Value;
        var dollEnt = _characterProfileSystem.CreateProfileDoll(editorControl.LiveProfile, editorControl.PreviewMode);
        editorControl.PreviewEntity = (dollEnt, Comp<SpriteComponent>(dollEnt));
        RaiseUIEvent(new CharacterEditorPreviewChangedUIEvent(editorControl.PreviewEntity.Value));
        return editorControl.PreviewEntity.Value;
    }

    public void StartEditingSlot(CharacterEditorControl editorControl, int slot)
     {
         if (!_characterProfileSystem.TryGetCharacterProfile(slot, out var profile))
         {
             Log.Error($"Tried to start editing slot:{slot} but it doesn't have a profile!");
             return;
         }
         if (editorControl.LiveProfile != null)
         {
             if (editorControl.LiveProfile.Slot == slot)
                 return;
         }
         else ClearPreviewEntity(editorControl);
         editorControl.LiveProfile = new CharacterProfile(profile.GetData(false)) { Slot = slot };
         editorControl.PreviewEntity = EnsurePreviewEntity(editorControl);
         RefreshPreviewVisuals(editorControl);
         RaiseUIEvent(new CharacterEditorPreviewChangedUIEvent(editorControl.PreviewEntity.Value));
         RaiseUIEvent(new CharacterEditorProfileDirtiedUIEvent(editorControl.LiveProfile));
     }

         public void RefreshPreviewVisuals(CharacterEditorControl editorControl)
     {
         if (editorControl.LiveProfile == null)
            ClearPreviewEntity(editorControl);

         editorControl.PreviewEntity = EnsurePreviewEntity(editorControl);
     }

     public void ClearPreviewEntity(CharacterEditorControl editorControl)
     {
         if (editorControl.PreviewEntity != null)
         {
             EntityManager.DeleteEntity(editorControl.PreviewEntity);
             editorControl.PreviewEntity = null;
         }
     }

}


// public sealed partial class CharacterEditorSystem : UISystem
// {
//     [Dependency] private readonly CharacterProfileSystem _characterProfileSystem = default!;
//
//     public CharacterProfile? LiveProfile { get; private set; }= null;
//     private Entity<SpriteComponent>? _livePreview = null;
//     private CharacterPreviewMode _previewmode = default;
//
//     public bool HasLiveProfile => _livePreview != null;
//     public bool ProfileHasChanges => LiveProfile is { HasDirtyData: true };
//
//     public override void Initialize()
//     {
//         SubscribeUIEvent<ChangeCharacterEditorPreviewModeUIEvent>(HandlePreviewModeChange);
//         SubscribeUIEvent<SelectCharacterProfileUIEvent>(HandleCharacterSelected);
//         SubscribeUIRequest<EditCharacterProfileFieldUIRequest>(HandleEditProfileRequest);
//         SubscribeUIEvent<CharacterEditorProfileDirtiedUIEvent>(HandleProfileDirtied);
//     }
//
//     //TODO: handling legacy profile data conversions, remove this eventually
//
//     private void HandleProfileDirtied(ref readonly CharacterEditorProfileDirtiedUIEvent args)
//     {
//         var legacyProfile = args.Profile.GetData<LegacyCharacterData>().LegacyProfile;
//         var identityData = args.Profile.GetData<CharacterIdentityData>();
//         var speciesData = args.Profile.GetData<CharacterSpeciesData>();
//         legacyProfile = legacyProfile.WithName(identityData.Name);
//         legacyProfile = legacyProfile.WithAge(identityData.PhysicalAge);
//         legacyProfile = legacyProfile.WithGender(identityData.Gender);
//         legacyProfile = legacyProfile.WithSex(identityData.BodyType);
//         legacyProfile = legacyProfile.WithSpecies(speciesData.BaseSpecies);
//         legacyProfile = legacyProfile.WithCustomSpecieName(speciesData.CustomSpeciesName);
//         args.Profile.SetData(new LegacyCharacterData{LegacyProfile =  legacyProfile});
//         ClearPreviewEntity();
//         RefreshPreviewVisuals();
//     }
//
//     private void HandleEditProfileRequest(ref EditCharacterProfileFieldUIRequest args)
//     {
//         args.Profile = LiveProfile;
//     }
//
//     private void HandleCharacterSelected(ref readonly SelectCharacterProfileUIEvent args)
//     {
//         if (LiveProfile != null)
//         {
//             if (LiveProfile.Slot == args.Slot)
//                 return;
//             ClearPreviewEntity();
//             LiveProfile = null;
//         }
//         StartEditingSlot(args.Slot);
//     }
//
//     private void HandlePreviewModeChange(ref readonly ChangeCharacterEditorPreviewModeUIEvent args)
//     {
//         ChangePreviewMode(args.NewMode);
//     }
//
//     public void ShutdownEditor()
//     {
//         ClearPreviewEntity();
//         if (LiveProfile == null)
//             return;
//
//         LiveProfile = null;
//     }
//
//     public void StartEditingSlot(int slot)
//     {
//         if (!_characterProfileSystem.TryGetCharacterProfile(slot, out var profile))
//         {
//             Log.Error($"Tried to start editing slot:{slot} but it doesn't have a profile!");
//             return;
//         }
//         if (LiveProfile != null)
//         {
//             if (LiveProfile.Slot == slot)
//                 return;
//         }
//         else ClearPreviewEntity();
//         LiveProfile = new CharacterProfile(profile.GetData(false)) { Slot = slot };
//         _livePreview = EnsureLivePreview(_previewmode);
//         RefreshPreviewVisuals();
//         RaiseUIEvent(new CharacterEditorPreviewChangedUIEvent(_livePreview.Value));
//         RaiseUIEvent(new CharacterEditorProfileDirtiedUIEvent(LiveProfile));
//     }
//
//     public void ApplyChanges()
//     {
//         if (LiveProfile == null)
//         {
//             Log.Warning($"Tried to apply changes when no profile was being edited!");
//             return;
//         }
//         if (!ProfileHasChanges)
//             return;
//         if (!_characterProfileSystem.TryGetCharacterProfile(LiveProfile.Slot, out var profile))
//         {
//             Log.Error($"Could not find profile for slot {LiveProfile.Slot} to apply changes!");
//             return;
//         }
//         profile.SetData(LiveProfile.GetData());
//         _characterProfileSystem.ApplyProfileChanges(LiveProfile.Slot);
//         //RaiseUIEvent(new CharacterEditor(_liveProfile));
//     }
//
//
//     public void DiscardChanges()
//     {
//         if (LiveProfile == null)
//             return;
//         if (!_characterProfileSystem.TryGetCharacterProfile(LiveProfile.Slot, out var profile))
//         {
//             LiveProfile = null;
//             return;
//         }
//         LiveProfile.SetData(profile.GetData());
//         ClearPreviewEntity();
//         RefreshPreviewVisuals();
//         //RaiseUIEvent(new CharacterEditingUpdateUIEvent(_liveProfile));
//     }
//
//     public void ChangePreviewMode(CharacterPreviewMode newPreviewMode)
//     {
//         if (_previewmode == newPreviewMode)
//             return;
//         _previewmode = newPreviewMode;
//         ClearPreviewEntity();
//         RefreshPreviewVisuals();
//     }
//
//     private Entity<SpriteComponent> EnsureLivePreview(CharacterPreviewMode previewMode = default)
//     {
//         if (LiveProfile == null)
//             throw new InvalidOperationException("Cannot ensure preview without a live profile!");
//         if (_livePreview != null)
//             return _livePreview.Value;
//         var dollEnt = _characterProfileSystem.CreateProfileDoll(LiveProfile, previewMode);
//         _livePreview = (dollEnt, Comp<SpriteComponent>(dollEnt));
//         RaiseUIEvent(new CharacterEditorPreviewChangedUIEvent(_livePreview.Value));
//         return _livePreview.Value;
//     }
//
//     public void RefreshPreviewVisuals()
//     {
//         if (LiveProfile == null)
//            ClearPreviewEntity();
//
//         _livePreview = EnsureLivePreview(_previewmode);
//     }
//
//     public void ClearPreviewEntity()
//     {
//         if (_livePreview != null)
//         {
//             EntityManager.DeleteEntity(_livePreview);
//             _livePreview = null;
//         }
//     }
//
//     public void SetLiveData<TProfileData, TValue>(TValue value,CharacterDataSetterDelegate<TProfileData, TValue> setter)
//         where TProfileData : struct, ICharacterData
//     {
//         LiveProfile?.EditData(value, setter);
//     }
//
//     public void SetLiveData<TData>(TData data, bool dirty = true) where TData : struct, ICharacterData
//     {
//         LiveProfile?.SetData(data, dirty);
//     }
// }