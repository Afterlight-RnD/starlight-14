// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Robust.Shared.Configuration;

namespace Content.Shared.Starlight.CCVar;
public sealed partial class StarlightCCVars
{

    /// <summary>
    /// Fallback job to use when no job is found
    /// </summary>
    public static readonly CVarDef<string> FallbackJob =
        CVarDef.Create("jobs.fallback_job", "Assistant",CVar.SERVER);
}