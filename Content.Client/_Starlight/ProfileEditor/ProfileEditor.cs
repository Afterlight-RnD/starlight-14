// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT
using Content.Shared._Starlight.CharacterProfiles;


namespace Content.Client._Starlight.ProfileEditor;


public sealed class ProfileEditor<TProfile>()
where TProfile: class, IPersistentProfile, new()
{
    public TProfile? EditingProfile { get; private set; }
}