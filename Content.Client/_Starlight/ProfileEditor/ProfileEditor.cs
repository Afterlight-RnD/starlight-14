// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Diagnostics.Contracts;
using Content.Client._Starlight.ProfileEditor.UI;
using Content.Client._Starlight.UI;
using Content.Client._Starlight.UI.Core;
using Content.Shared._Starlight.Abstract.Extensions;
using Content.Shared._Starlight.CharacterProfiles;
using Robust.Client.UserInterface;


namespace Content.Client._Starlight.ProfileEditor;

public interface IProfileEditor<TSelf>
    where TSelf: IProfileEditor<TSelf>, new()
{
    public static UIEventHandle SubscribeEditorEvent<TControl, TEvent>(UIEvent<TControl,TEvent> handler)
        where TEvent : struct
        where TControl: Control, new()
    {
        return UIEvents.Subscribe(
            delegate(TControl control, ref readonly UIEventWrapper<TSelf, TEvent> args)
            {
                args.ForwardControlArgs(control,handler);
            });
    }

    public static UIEventHandle SubscribeEditorEvent<TEvent>(UIEvent<TEvent> handler)
        where TEvent : struct
    {
        return UIEvents.Subscribe(
            delegate(ref readonly UIEventWrapper<TSelf, TEvent> args)
            {
                args.ForwardArgs(handler);
            });
    }
    public void RaiseEditorEvent<TEvent>(TEvent args)
        where TEvent : struct;

    public void RaiseControlEditorEvent<TEvent>(TEvent args)
        where TEvent : struct;

    public struct UIEventWrapper<TProfileEditor, TEvent>(
        TProfileEditor editor,
        TEvent args)
        where TEvent : struct
        where TProfileEditor : IProfileEditor<TProfileEditor>, new()
    {
        public TEvent Args = args;
        public TProfileEditor Editor = editor;

        [Pure]
        public void ForwardArgs(UIEvent<TEvent> handler)
        {
            handler.Invoke(ref Args);
        }

        [Pure]
        public void ForwardControlArgs<TControl>(TControl control,UIEvent<TControl, TEvent> handler)
            where TControl: Control, new()
        {
            handler.Invoke(control, ref Args);
        }
    };

}

public interface IProfileEditor<TSelf,TEditorControl> : IProfileEditor<TSelf>
    where TSelf: IProfileEditor<TSelf,TEditorControl>, new()
    where TEditorControl: SLControl, IProfileEditorMainControl<TEditorControl,TSelf>
{
    public void Initialize(TEditorControl editorControl);

    public TEditorControl EditorControl { get; }

    public bool IsOpen => EditorControl.IsInsideTree;
}

public abstract class ProfileEditor<TSelf,TProfile, TEditorControl> :  IProfileEditor<TSelf,TEditorControl>
where TSelf: ProfileEditor<TSelf,TProfile, TEditorControl> , new()
where TProfile: class, IPersistentProfile, new()
where TEditorControl: SLControl, IProfileEditorMainControl<TEditorControl,TSelf>
{
    [Dependency] protected readonly IEntityManager EntityManager = default!;

    public TProfile? EditingProfile { get; private set; }
    public TEditorControl EditorControl { get; private set; } = default!;

    public void Initialize(TEditorControl editorControl)
    {
        EditorControl = editorControl;
        if (!EntityManager.IsInitialized())
            throw new InvalidOperationException($"Tried to create profile editor:{GetType()} outside of sim!");
    }

    public void RaiseEditorEvent<TEvent>(TEvent args)
    where TEvent: struct
    {
        UIEvents.RaiseEvent(new IProfileEditor<TSelf>.UIEventWrapper<TSelf, TEvent>((TSelf)this, args));
    }

    public void RaiseControlEditorEvent<TEvent>(TEvent args)
        where TEvent: struct
    {
        UIEvents.RaiseControlEventRecursive(EditorControl,new IProfileEditor<TSelf>.UIEventWrapper<TSelf,TEvent>((TSelf)this, args));
    }
}
