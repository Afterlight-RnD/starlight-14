// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Diagnostics.CodeAnalysis;
using Robust.Client.UserInterface;

namespace Content.Client._Starlight.UI.Controls;

[Virtual]
public class SingletonControlStack<TKey> : ControlStack<TKey> where TKey: struct, Enum
{
    public TKey StartingKey { get; init; } = new();

    private StackEntry? _activeStackEntry = null;

    public override void SetVisibilityInStack(TKey key, bool newVisible)
    {
        if (StackedControls.TryGetValue(key, out var entry))
            entry.Visible = newVisible;
        if (_activeStackEntry != null)
        {
            if (_activeStackEntry.Key.Equals(key))
                return;
            _activeStackEntry.Visible = false;
        }
        _activeStackEntry = entry;
    }

    public bool TryGetActiveStackedControl<T>([NotNullWhen(true)] out T? foundControl) where T:Control, new()
    {
        foundControl = null;
        if (_activeStackEntry is not T control)
            return false;
        foundControl = control;
        return true;
    }

    protected override void ChildAdded(Control newChild)
    {
        base.ChildAdded(newChild);
        var childEntry = (StackEntry)newChild;
        if (_activeStackEntry == null && childEntry.Key.Equals(StartingKey))
            _activeStackEntry = childEntry;
    }

    protected override void ChildRemoved(Control child)
    {
        if (child is not StackEntry entry)
        {
            base.ChildRemoved(child);
            return;
        }
        StackedControls.Remove(entry.Key);
        base.ChildRemoved(child);
    }
}