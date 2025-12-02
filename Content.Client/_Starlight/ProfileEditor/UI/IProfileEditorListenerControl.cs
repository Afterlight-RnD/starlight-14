// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.CharacterProfiles;

namespace Content.Client._Starlight.ProfileEditor.UI;
public interface IProfileEditorListenerControl<in TProfile>
where TProfile: class,IPersistentProfile
{
    public void InjectProfile(TProfile profile);
}