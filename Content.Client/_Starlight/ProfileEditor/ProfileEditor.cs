// SPDX-FileCopyrightText: 2025 Starlight Network
// SPDX-License-Identifier: Starlight-MIT

using Content.Client._Starlight.ProfileEditor.UI;
using Content.Shared._Starlight.Abstract.Extensions;
using Content.Shared._Starlight.Abstract.Interfaces;
using Content.Shared._Starlight.CharacterProfiles;
using Robust.Shared.Collections;

namespace Content.Client._Starlight.ProfileEditor;

public interface IProfileEditor<TSelf, TProfile>
    where TSelf: IProfileEditor<TSelf, TProfile>
    where TProfile: class, IPersistentProfile
{
    public TProfile Profile { get; }
}

public abstract class ProfileEditor<TSelf,TProfile, TEditorControl, TPanelEnum> : IProfileEditor<TSelf, TProfile>, IInjectDependencies<SystemDependencies>
    where TSelf: ProfileEditor<TSelf,TProfile, TEditorControl, TPanelEnum>
    where TProfile : class, IPersistentProfile
    where TEditorControl: ProfileEditorMainControl<TEditorControl,TProfile, TSelf, TPanelEnum>, new()
    where TPanelEnum: struct, Enum
{
    private ValueList<ProfileEditorStep<TProfile>> _steps;
    public TEditorControl EditorControl { get; }
    public TProfile Profile { get; }

    protected ProfileEditor( IDynamicTypeFactory typeFactory,
        IDependencyCollection dependencyCollection,
        TProfile profile,
        BuilderDelegate builder)
    {
        var editorCollection = dependencyCollection.FromParent(dependencyCollection);
        Profile = profile;
        editorCollection.RegisterInstance<TProfile>(Profile);
        editorCollection.RegisterInstance<TSelf>(this);

        EditorControl = typeFactory.CreateInstance<TEditorControl, SystemDependencies>(editorCollection);
        editorCollection.RegisterInstance<TEditorControl>(EditorControl);

        var stepBuilder = new StepStepBuilder();
        builder.Invoke(stepBuilder);
        if (stepBuilder.Builders.Count == 0)
            throw new InvalidOperationException($"profileEditor: {GetType()} must have at least one step!");
        foreach (var (name, priority,builderDelegate) in stepBuilder.Builders)
        {
            var step = ProfileEditorStep<TProfile>.BuildEditorStep(typeFactory, dependencyCollection, name, priority, builderDelegate);
            _steps.Add(step);
            EditorControl.INTERNAL_InjectPanels(typeFactory, dependencyCollection, stepBuilder.PanelRegistrations);
        }
        //Sort steps by ascending!
        _steps.Sort((step1, step2) =>
        {
            if (step1.Priority < step2.Priority)
                return -1;
            return step1.Priority > step2.Priority ? 1 : 0;
        });
    }

    #region BuilderPattern
    public interface IStepBuilder
    {
        public IStepBuilder BuildStep(string name, int priority, ProfileEditorStep<TProfile>.BuilderDelegate builder);

        public IStepBuilder RegisterPanel<TPanel>(TPanelEnum panelEnum);
    }

    private struct StepStepBuilder() : IStepBuilder
    {
        public ValueList<(string name, int priority, ProfileEditorStep<TProfile>.BuilderDelegate buildDelegate)>
            Builders = new ();
        public HashSet<(TPanelEnum, Type)> PanelRegistrations = new();
        public IStepBuilder BuildStep(string name, int priority, ProfileEditorStep<TProfile>.BuilderDelegate builder)
        {
            Builders.Add((name, priority, builder));
            return this;
        }
        public IStepBuilder RegisterPanel<TPanel>(TPanelEnum panelEnum)
        {
            PanelRegistrations.Add((panelEnum, typeof(TPanel)));
            return this;
        }
    }
    public delegate void BuilderDelegate(IStepBuilder stepBuilder);
    #endregion
}