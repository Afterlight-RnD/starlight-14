// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.CharacterProfiles;

namespace Content.Client._Starlight.ProfileEditor.UI;

public interface IProfileEditorField
{
}

public interface IProfileEditorField<TData, TProfile> : IProfileEditorField
where TProfile: IPersistentProfile, new()
{
    public IProfileEditor<TProfile> Editor { get; }
    public void FromProfile(TProfile data);

    public void ToProfile(TProfile data);
}
