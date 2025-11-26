// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.UI.Core;
using Robust.Client.Graphics;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.Controls;
using Robust.Shared.Collections;

namespace Content.Client._Starlight.UI.Controls;


public abstract class SLOptionButton<TData> : OptionButton, ISLControl
{
    public virtual string? LocPrefix { get; init; } = null;
    public event Action<TData>? OnDataSelected = null;

    public TData CurrentOption => _optionData[SelectedId];

    public int OptionCount => _optionData.Count;

    private bool _optionsSetup = false;

    private ValueList<TData> _optionData = new();
    public event Action<Control>? OnControlEnteredTree;
    public event Action<Control>? OnControlExitedTree;

    public abstract IEnumerable<TData> EnumerateOptions();

    public virtual void DataAdded(TData data, int id){}


    [MustCallBase(true)]
    protected virtual void ItemSelected(TData item){}

    protected abstract string GetOptionLabel(TData data);

    protected virtual Texture? GetOptionIcon(TData data) { return null;}

    [MustCallBase]
    protected override void EnteredTree()
    {
        SetupOptions();
        OnControlEnteredTree?.Invoke(this);
    }

    [MustCallBase]
    protected override void ExitedTree()
    {
        OnControlExitedTree?.Invoke(this);
        ClearOptions();
        UnsubscribeAllUIEvents();
    }

    public void SetupOptions()
    {
        if (_optionsSetup)
            ClearOptions();
        foreach (var option in EnumerateOptions())
        {
            var optionId = _optionData.Count;
            _optionData.Add(option);
            DataAdded(option, optionId);
            var optionIcon = GetOptionIcon(option);

            string optionLabel;
            optionLabel = LocPrefix == null
                ? GetOptionLabel(option)
                : Loc.GetString($"{LocPrefix}-{GetOptionLabel(option).ToLower()}");

            if (optionIcon != null)
            {
                AddItem(optionIcon, optionLabel, optionId);
            }
            else
            {
                AddItem(optionLabel, optionId);
            }
            OnItemSelected += HandleItemSelected;
            if (optionId == 0)
                Select(0);
        }
        _optionsSetup = true;
    }

    public void ClearOptions()
    {
        if (!_optionsSetup)
            return;
        Clear();
        _optionData.Clear();
        _optionsSetup = false;
    }

    public void SelectByData(TData data)
    {
        var id = GetOptionId(data);
        if (id == -1)
            return;
        SelectId(id);
    }

    public int GetOptionId(TData data)
    {
        for (var i = 0; i < _optionData.Count; i++)
        {
            var entry = _optionData[i];
            if (EqualityComparer<TData>.Default.Equals(entry, data))
                return i;
        }
        return -1;
    }

    private void HandleItemSelected(ItemSelectedEventArgs args)
    {
        SelectId(args.Id);
        var data = _optionData[args.Id];
        ItemSelected(data);
        OnDataSelected?.Invoke(data);
    }

    #region UIEvents

    private HashSet<UIEventHandle> _uiEventHandles { get; } = new();

    public void SubscribeUIRequest<T>(UIRequest<T> handler) where T : struct
    {
        _uiEventHandles.Add(UIEvents.SubscribeRequest(handler));
    }

    public void SubscribeUIEvent<T>(UIEvent<T> handler) where T : struct
    {
        _uiEventHandles.Add(UIEvents.Subscribe(handler));
    }

    public void RaiseUIEvent<T>(T args) where T : struct
    {
        UIEvents.RaiseEvent(args);
    }

    public void RaiseRequest<T>(ref T args) where T : struct
    {
        UIEvents.RaiseRequest(ref args);
    }

    public void UnsubscribeUIEvent(ref UIEventHandle handle)
    {
        //EventType is never null if handle is valid
        if (!handle.IsValid || _uiEventHandles.Remove(handle))
            return;
        handle.Unsubscribe();
    }

    public void UnsubscribeAllUIEvents()
    {
        foreach (var handle in _uiEventHandles)
        {
            handle.Unsubscribe();
        }

        _uiEventHandles.Clear();
    }

    #endregion
}