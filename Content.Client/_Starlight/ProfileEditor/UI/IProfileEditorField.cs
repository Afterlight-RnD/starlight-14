// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.CharacterProfiles;

namespace Content.Client._Starlight.ProfileEditor.UI;

public interface IProfileEditorField
{
}

public interface IProfileEditorField<TProfile> : IProfileEditorField
    where TProfile : class, IPersistentProfile
{
    public virtual void SetEditWidth(int width){}
    public void InjectProfile(TProfile profile);
}
public interface IProfileEditorField<TData, TProfile> : IProfileEditorField<TProfile>
where TProfile: class, IPersistentProfile<TProfile>
{

    public void FromProfile(TProfile profile);

    public void ToProfile();
}
