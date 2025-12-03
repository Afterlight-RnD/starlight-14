// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Shared._Starlight.Abstract.Extensions;
using Content.Shared._Starlight.Abstract.Interfaces;
using Content.Shared._Starlight.CharacterProfiles;
using Robust.Client.Graphics;

namespace Content.Client._Starlight.ProfileEditor;

public sealed class ProfileEditorStep<TProfile> : IInjectDependencies<SystemDependencies>
    where TProfile : class, IPersistentProfile
{
    public int Priority { get; private set; } = 0;
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; } = null;
    public Texture? Icon { get; private set; } = null;

    public interface IBuilder
    {
        public string? Description { set; }
        public Texture? Icon {set; }
    }

    protected sealed class Builder : IBuilder, IInjectDependencies<SystemDependencies>
    {
        public string? Description { get; set; }
        public Texture? Icon { get; set; }

        public ProfileEditorStep<TProfile> Finalize(IDynamicTypeFactory typeFactory, IDependencyCollection collection)
        {
            var step = typeFactory.CreateInstance<ProfileEditorStep<TProfile>, SystemDependencies>(collection);
            return step;
        }
    }

    #region BuilderPattern
    public static ProfileEditorStep<TProfile> BuildEditorStep(
        IDynamicTypeFactory typeFactory,
        IDependencyCollection dependencyCollection,
        string name,
        int priority,
        BuilderDelegate build)
    {
        var builder = typeFactory.CreateInstance<Builder,SystemDependencies>(dependencyCollection);
        build.Invoke(builder);
        var step = builder.Finalize(typeFactory, dependencyCollection);
        step.Name = name;
        step.Priority = priority;
        return step;
    }
    public delegate void BuilderDelegate(IBuilder builder);
    #endregion

}