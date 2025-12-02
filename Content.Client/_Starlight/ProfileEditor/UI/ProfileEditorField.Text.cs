// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.CharacterProfiles;
using Robust.Client.UserInterface.Controls;

namespace Content.Client._Starlight.ProfileEditor.UI;

public sealed class ProfileEditorTextField<TProfile>(
    IProfileEditor<TProfile> editor,
    Func<TProfile, string> readData,
    Action<TProfile, string> writeData)
    : LineEdit, IProfileEditorField<string, TProfile>
    where TProfile : IPersistentProfile, new()
{
    public IProfileEditor<TProfile> Editor { get; } = editor;

    public void SetData(string data) => SetText(data);

    public string GetData() => Text;

    public void FromProfile(TProfile data)
    {
        SetText(readData.Invoke(data), false);
    }

    public void ToProfile(TProfile data) => writeData.Invoke(data, Text);
}