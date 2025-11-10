// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

namespace Content.Client._Starlight.UI.Controls;

[Virtual]
public class SLEnumOptionButton<TEnum> :SLOptionButton<TEnum> where TEnum: struct, Enum
{
    public override IEnumerable<TEnum> EnumerateOptions()
    {
        foreach (var value in Enum.GetValues<TEnum>())
            yield return value;
    }

    protected override string GetOptionLabel(TEnum data)
    {
        return data.ToString();
    }
}