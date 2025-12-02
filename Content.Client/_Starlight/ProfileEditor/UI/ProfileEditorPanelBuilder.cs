// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.CharacterProfiles;
using Robust.Client.UserInterface;

namespace Content.Client._Starlight.ProfileEditor.UI;

public interface IProfileEditorPanelBuilder;

public interface IProfileEditorPanelBuilder<TProfile> : IProfileEditorPanelBuilder
    where TProfile: IPersistentProfile, new()
{
    public void RegisterField<TField>(ProfileEditorField<TProfile> fieldControl, TField field)
        where TField : Control, IProfileEditorField<TProfile>;
}

public sealed class ProfileEditorPanelBuilder<TEditor,TProfile,TLayoutEnum, TEditorControl, TStepSelectorButton> : IProfileEditorPanelBuilder<TProfile>
where TProfile: IPersistentProfile, new()
where TEditorControl: ProfileEditorMainControl<TEditor, TLayoutEnum, TStepSelectorButton>
where TEditor: IProfileEditor, new()
where TLayoutEnum: struct, Enum, IConvertible
where TStepSelectorButton: ProfileEditorStepButton, new()
{

    private Dictionary<TLayoutEnum, (Type, Action<Control, IProfileEditorPanelBuilder<TProfile>>)> _panelTypes = new();

    private ISawmill _log;
    private readonly IDynamicTypeFactory _typeFactory;

    public ProfileEditorPanelBuilder(ISawmill log, IDynamicTypeFactory typeFactory)
    {
        _log = log;
        _typeFactory = typeFactory;
    }

    public void InjectPanels(IDynamicTypeFactory typeFactory,
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
            fieldInjector.Invoke(newControl, this);
            injectionHandler.Invoke(layout, newControl);
        }
    }

    public void RegisterPanel<TPanel>(TLayoutEnum layout,
        Action<TPanel, IProfileEditorPanelBuilder<TProfile>> setupFields)
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