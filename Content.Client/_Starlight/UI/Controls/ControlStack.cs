// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using System.Diagnostics.CodeAnalysis;
using Robust.Client.UserInterface;

namespace Content.Client._Starlight.UI.Controls;

[Virtual]
public class ControlStack<TKey> : Control where TKey: Enum, new()
{
    public bool StackedControlsStartVisible { get; init; } = true;

    protected readonly Dictionary<TKey, StackEntry> StackedControls = new();

    protected ControlStack()
    {
        HorizontalExpand = true;
        VerticalExpand = true;
        HorizontalAlignment = HAlignment.Stretch;
        VerticalAlignment = VAlignment.Stretch;
    }

    public virtual void SetVisibilityInStack(TKey key, bool newVisible)
    {
        if (StackedControls.TryGetValue(key, out var value))
            value.Visible = newVisible;
    }

    public bool SetPositionInStack(TKey key, int newPos)
    {
        if (!StackedControls.TryGetValue(key, out var entry)) return true;
        if (newPos >= ChildCount || newPos < 0)
            return false;
        entry.SetPositionInParent(newPos);
        return true;
    }

    protected override void ChildAdded(Control newChild)
    {
        if (newChild is not StackEntry entry)
            throw new InvalidOperationException($"ControlStack:{GetType()} only support children of type:{typeof(StackEntry)}");
        if (!StackedControls.TryAdd(entry.Key, entry))
            throw new InvalidOperationException($"ControlStack:{GetType()} already has control registered for key:{entry.Key}");
        if (!StackedControlsStartVisible)
            entry.Visible = false;
        base.ChildAdded(newChild);
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

    public T GetStackedControl<T>(TKey key) where T:Control, new()
    {
        if (StackedControls[key] is T control)
            return control;
        throw new KeyNotFoundException($"Could not find ControlEntry of type:{typeof(T)} for key:{key} StackedControl:{this}");
    }

    public bool TryGetStackedControl<T>(TKey key, [NotNullWhen(true)] out T? foundControl) where T : Control, new()
    {
        foundControl = null;
        return StackedControls.TryGetValue(key, out var stackEntry) && stackEntry.TryGetGetWrappedType(out foundControl);
    }

    public sealed class StackEntry : Control
    {
        public TKey Key { get; init; } = new();
        public StackEntry()
        {
            HorizontalExpand = true;
            VerticalExpand = true;
            HorizontalAlignment = HAlignment.Stretch;
            VerticalAlignment = VAlignment.Stretch;
        }

        public T GetWrappedType<T>() where T : Control
        {
            var child = Children.GetEnumerator().Current;
            return (T)child;
        }

        public bool TryGetGetWrappedType<T>([NotNullWhen(true)] out T? foundControl) where T : Control, new()
        {
            foundControl = null;
            if (ChildCount == 0)
                return false;
            var child = Children.GetEnumerator().Current;
            if (child is not T entry)
                return false;
            foundControl = entry;
            return true;
        }
    }
}