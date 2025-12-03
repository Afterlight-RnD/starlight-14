// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT
using Content.Client._Starlight.UI;
using Content.Shared._Starlight.Abstract.Extensions;
using Content.Shared._Starlight.Abstract.Interfaces;
using Content.Shared._Starlight.CharacterProfiles;
using Robust.Client.UserInterface;

namespace Content.Client._Starlight.ProfileEditor.UI;
public abstract class ProfileEditorMainControl<TSelf,TProfile, TProfileEditor, TPanelEnum> :
    SLControl, IInjectDependencies<SystemDependencies>
    where TProfile: class, IPersistentProfile
    where TProfileEditor: ProfileEditor<TProfileEditor,TProfile, TSelf, TPanelEnum>
    where TPanelEnum: struct, Enum
    where TSelf: ProfileEditorMainControl<TSelf,TProfile, TProfileEditor, TPanelEnum>, new()
{
    [Dependency] protected TProfileEditor ProfileEditor = default!;

    private Dictionary<(TPanelEnum, Type), Control> _panelRegistry = new();
    protected abstract void InjectStepControl(TPanelEnum panelEnum, Control control);

    public void INTERNAL_InjectPanels(
        IDynamicTypeFactory typeFactory,
        IDependencyCollection deps,
        HashSet<(TPanelEnum, Type)> panelRegistrations)
    {
        foreach (var panelKey in panelRegistrations)
        {
            if (_panelRegistry.ContainsKey(panelKey))
                continue;
            var control = typeFactory.CreateCastInstance<Control, SystemDependencies>(panelKey.Item2, deps);
            control.Visible = false;
            _panelRegistry.Add(panelKey, control);
            InjectStepControl(panelKey.Item1, control);
        }
    }
}