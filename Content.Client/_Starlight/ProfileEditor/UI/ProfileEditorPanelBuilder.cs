// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.CharacterProfiles;
using Robust.Client.UserInterface;

namespace Content.Client._Starlight.ProfileEditor.UI;

public interface IProfileEditorPanelBuilder;

public partial interface IProfileEditorFieldBuilder<TProfile> : IProfileEditorPanelBuilder
    where TProfile: class, IPersistentProfile<TProfile>
{
    public void RegisterField<TField>(ProfileEditorField<TProfile> fieldControl, TField field)
        where TField : Control, IProfileEditorField<TProfile>;
}

public interface IProfileEditorPanelBuilder<TProfile,TLayoutEnum> : IProfileEditorFieldBuilder<TProfile>
    where TProfile: class, IPersistentProfile<TProfile>
    where TLayoutEnum: struct, Enum, IConvertible
{
    public void RegisterPanel<TPanel>(TLayoutEnum layout,
        Action<TPanel, IProfileEditorFieldBuilder<TProfile>> setupFields)
        where TPanel : Control, new();
}

public interface IProfileEditorPanelInjector<TProfile, out TLayoutEnum>:  IProfileEditorPanelBuilder
    where TProfile: class, IPersistentProfile
    where TLayoutEnum: struct, Enum, IConvertible
{
    public void InjectPanels(TProfile profile,
        int step,
        ref Control?[,] stepControls,
        Action<TLayoutEnum, Control> injectionHandler);
}

public sealed partial class ProfileEditorPanelBuilder<TEditor,TProfile,TLayoutEnum, TEditorControl, TStepSelectorButton> : IProfileEditorPanelInjector<TProfile, TLayoutEnum>, IProfileEditorPanelBuilder<TProfile,TLayoutEnum>
where TProfile: class, IPersistentProfile<TProfile>
where TEditorControl: ProfileEditorMainControl<TEditor, TProfile,TLayoutEnum, TStepSelectorButton>
where TEditor: IProfileEditor<TProfile>, new()
where TLayoutEnum: struct, Enum, IConvertible
where TStepSelectorButton: ProfileEditorStepButton, new()
{

    private Dictionary<TLayoutEnum, (Type, Action<Control, IProfileEditorFieldBuilder<TProfile>>)> _panelTypes = new();

    private ISawmill _log;
    private readonly IDynamicTypeFactory _typeFactory;

    public ProfileEditorPanelBuilder(ISawmill log, IDynamicTypeFactory typeFactory)
    {
        _log = log;
        _typeFactory = typeFactory;
    }

    public void InjectPanels(TProfile profile,
        int step,
        ref Control?[,] stepControls,
        Action<TLayoutEnum,Control> injectionHandler)
    {
        foreach (var (layout, (type,fieldInjector)) in _panelTypes)
        {
            var layoutId = layout.ToInt32(null);
            var existing = stepControls[step,  layoutId];
            if (existing != null)
                continue;
            var newControl = _typeFactory.CreateInstance<Control>(type);
            newControl.Visible = false;
            stepControls[step, layoutId] = newControl;
            ControlRecursivelyInjectProfile(profile, newControl);
            fieldInjector.Invoke(newControl, this);
            injectionHandler.Invoke(layout, newControl);
        }
    }

    private void ControlRecursivelyInjectProfile(TProfile profile, Control control)
    {
        foreach (var child in control.Children)
            ControlRecursivelyInjectProfile(profile, child);
        if (control is IProfileEditorListenerControl<TProfile> listener)
            listener.InjectProfile(profile);
    }

    public void RegisterPanel<TPanel>(TLayoutEnum layout,
        Action<TPanel, IProfileEditorFieldBuilder<TProfile>> setupFields)
        where TPanel : Control, new()
    {
        if (_panelTypes.TryAdd(layout, (typeof(TPanel), (control, builder) => setupFields.Invoke((TPanel)control, builder))))
            _log.Warning("Panel was already registered!");
    }

    public void RegisterField<TField>(ProfileEditorField<TProfile> fieldControl, TField field)
        where TField: Control, IProfileEditorField<TProfile>
    {
        fieldControl.InitializeAsFieldType(field);
    }
}