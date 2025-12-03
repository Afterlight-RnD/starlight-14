// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.Abstract.Interfaces;
using Content.Shared._Starlight.CharacterProfiles;

namespace Content.Client._Starlight.ProfileEditor.UI;

public interface IProfileEditorField<TEditor,TProfile> : IInjectDependencies<SystemDependencies>
    where TEditor: class, IProfileEditor<TEditor, TProfile>
    where TProfile : class, IPersistentProfile
{
    public void ReadFromProfile(TProfile profile);
    public void WriteToProfile(TProfile profile);
}