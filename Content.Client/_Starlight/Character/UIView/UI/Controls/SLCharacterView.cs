// SPDX-FileCopyrightText: 2025 Afterlight RnD
// SPDX-License-Identifier: ASL-1.2

using Content.Client._Starlight.Character.UIView.Systems;
using Content.Shared.Preferences;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;

namespace Content.Client._Starlight.Character.UIView.UI.Controls;

[Virtual]
public class SLCharacterView : SpriteView
{
    [Dependency] protected readonly IEntityManager EntityManager = default!;
    protected CharacterUIViewSystem CharacterViewSystem = default!;

    public bool HasView => Entity != null;
    public SLCharacterView()
    {
        IoCManager.InjectDependencies(this);
        CharacterViewSystem = EntityManager.System<CharacterUIViewSystem>();
    }

    public bool SetFromProfile(HumanoidCharacterProfile? characterProfile)
    {
        if (characterProfile == null)
        {
            ClearCharacter();
            return true;
        }
        if (Entity != null)
            return CharacterViewSystem.UpdateBaseView((Entity.Value, null, null), characterProfile);
        SetEntity(CharacterViewSystem.CreateBaseView(characterProfile));
        return true;
    }

    public void ClearCharacter()
    {
        SetEntity(null);
    }

    [MustCallBase(true)]
    protected override void ExitedTree()
    {
        ClearCharacter();
    }
}