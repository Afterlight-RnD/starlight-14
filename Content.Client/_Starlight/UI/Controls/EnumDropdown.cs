// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

namespace Content.Client._Starlight.UI.Controls;

public abstract class EnumDropdown<TEnum>(TEnum initialValue) : ValueDropdown<TEnum>(initialValue)
    where TEnum : struct, Enum
{
    public override string GetLocStringForValue(TEnum value) => value.ToString().ToLower();

    public override IEnumerable<TEnum> IterateValues() => Enum.GetValues<TEnum>();
}