// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Robust.Client.UserInterface;

namespace Content.Client._Starlight.UI.Core;

public sealed class ControlDependencyInjectionSystem : UISystem
{
    public override void Initialize()
    {
        RaiseUIEvent(new SystemsLoadedUIEvent(EntityManager.EntitySysManager.DependencyCollection));
    }
}
/// <summary>
/// Event that passes all SystemDependencies to subscribed UIControls when those dependencies are first initialized
/// </summary>
public record struct SystemsLoadedUIEvent(IDependencyCollection SystemDependencies);
