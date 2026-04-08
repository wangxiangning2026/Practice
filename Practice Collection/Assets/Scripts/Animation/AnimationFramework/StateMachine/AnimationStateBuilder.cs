using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 状态构建器（流畅接口）
/// </summary>
public class AnimationStateBuilder
{
    private readonly IAnimationPlayer player;
    private readonly IAnimationEventHandler eventHandler;
    private string name;
    private AnimationClip clip;
    private int layer;
    private AnimationPriority priority;
    private Action<object> onEnter;
    private Action<float> onUpdate;
    private Action onExit;
    private Func<IAnimationState, bool> canTransition;

    public AnimationStateBuilder(IAnimationPlayer player, IAnimationEventHandler eventHandler)
    {
        this.player = player;
        this.eventHandler = eventHandler;
    }

    public AnimationStateBuilder WithName(string name)
    {
        this.name = name;
        return this;
    }

    public AnimationStateBuilder WithClip(AnimationClip clip)
    {
        this.clip = clip;
        return this;
    }

    public AnimationStateBuilder OnLayer(int layer)
    {
        this.layer = layer;
        return this;
    }

    public AnimationStateBuilder WithPriority(AnimationPriority priority)
    {
        this.priority = priority;
        return this;
    }

    public AnimationStateBuilder OnEnter(Action<object> action)
    {
        onEnter = action;
        return this;
    }

    public AnimationStateBuilder OnUpdate(Action<float> action)
    {
        onUpdate = action;
        return this;
    }

    public AnimationStateBuilder OnExit(Action action)
    {
        onExit = action;
        return this;
    }

    public AnimationStateBuilder WithTransitionCondition(Func<IAnimationState, bool> condition)
    {
        canTransition = condition;
        return this;
    }

    public IAnimationState Build()
    {
        return new DynamicAnimationState(name, clip, layer, priority, player, eventHandler)
        {
            OnEnterAction = onEnter,
            OnUpdateAction = onUpdate,
            OnExitAction = onExit,
            CanTransitionFunc = canTransition
        };
    }

    private class DynamicAnimationState : BaseAnimationState
    {
        public Action<object> OnEnterAction;
        public Action<float> OnUpdateAction;
        public Action OnExitAction;
        public Func<IAnimationState, bool> CanTransitionFunc;

        public DynamicAnimationState(string name, AnimationClip clip, int layer, AnimationPriority priority,
            IAnimationPlayer player, IAnimationEventHandler eventHandler)
            : base(player, eventHandler)
        {
            Name = name;
            Clip = clip;
            Layer = layer;
            Priority = priority;
        }

        public new string Name { get; private set; }
        public new AnimationClip Clip { get; private set; }
        public new int Layer { get; private set; }
        public new AnimationPriority Priority { get; private set; }

        public override void OnEnter(object transitionData)
        {
            OnEnterAction?.Invoke(transitionData);
        }

        public override void OnUpdate(float deltaTime)
        {
            OnUpdateAction?.Invoke(deltaTime);
        }

        public override void OnExit()
        {
            OnExitAction?.Invoke();
        }

        public override bool CanTransitionTo(IAnimationState nextState)
        {
            if (CanTransitionFunc != null)
                return CanTransitionFunc(nextState);
            return base.CanTransitionTo(nextState);
        }
    }
}