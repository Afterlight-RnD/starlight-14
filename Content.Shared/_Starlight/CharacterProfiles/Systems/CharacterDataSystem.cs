// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

namespace Content.Shared._Starlight.CharacterProfiles.Systems;

public abstract class CharacterDataSystem<TData> : EntitySystem where TData: CharacterData
{
    protected virtual Type[]? ApplyBefore => null;

    protected virtual Type[]? ApplyAfter => null;

    public override void Initialize()
    {
        SubscribeLocalEvent<ApplyCharacterProfileEvent>(TryApply, ApplyBefore, ApplyAfter);
    }

    private void TryApply(ApplyCharacterProfileEvent ev)
    {
        if (ev.Data is TData data)
            Apply(ev.Target, data, ev.IsDoll);
    }

    protected abstract void Apply(EntityUid target, TData data, bool isDoll);
}

public abstract class CharacterDataSystem<TData, TComp> : EntitySystem
    where TData: CharacterData<TComp>
    where TComp:IComponent, new()
{
    protected virtual Type[]? ApplyBefore => null;

    protected virtual Type[]? ApplyAfter => null;

    public override void Initialize()
    {
        SubscribeLocalEvent<ApplyCharacterProfileEvent>(TryApply, ApplyBefore, ApplyAfter);
    }

    private void TryApply(ApplyCharacterProfileEvent ev)
    {
        if (ev.Data is not TData data
            || data.GetComponentType != typeof(TComp))
            return;
        var comp = EnsureComp<TComp>(ev.Target);
        Apply((ev.Target, comp), data, ev.IsDoll);
    }

    protected abstract void Apply(Entity<TComp> target, TData data, bool isDoll);
}