// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI;
using Robust.Client.ResourceManagement;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Reflection;

namespace Content.Client._Starlight.ProfileEditor.UI;



public abstract class ProfileEditorMainControl<TEditor> : SLControl
    where TEditor : class, IProfileEditor, new()
{
    [Dependency] private IReflectionManager _reflectionManager = default!;
    [Dependency] private IDynamicTypeFactory _typeFactory = default!;
    [Dependency] private ILogManager _logManager = default!;
    [Dependency] private IResourceCache _resourceCache = default!;

    public ButtonGroup StepSelectorGroup { get; }

    protected ProfileEditorMainControl()
    {
        IoCManager.InjectDependencies(this);
        StepSelectorGroup = new(false);
    }

    public abstract Control StepButtonRoot { get; }

    private TEditor? _editor = null;

    public TEditor Editor
    {
        get
        {
            if (_editor != null)
                return _editor;
            _editor =  _typeFactory.CreateInstance<TEditor>();
            List<Control> panels = new();
            foreach (var panelType in _reflectionManager.GetAllChildren(_editor.GetPanelBaseType))
            {
                panels.Add(_typeFactory.CreateInstance<Control>(panelType));
            }
            _editor.Initialize(panels, _resourceCache, _typeFactory,_logManager.GetSawmill("ProfileEditor"));
            return _editor;
        }
    }

}