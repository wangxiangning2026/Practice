using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 高级动画状态机
/// </summary>
public class AdvancedAnimationStateMachine : IAnimationStateMachine
{
    private readonly Dictionary<string, IAnimationState> states = new Dictionary<string, IAnimationState>();
    private readonly IAnimationPlayer player;
    private readonly IAnimationEventHandler eventHandler;

    private IAnimationState currentState;
    private IAnimationState previousState;
    private bool isInitialized;

    public IAnimationState CurrentState => currentState;
    public IAnimationState PreviousState => previousState;
    public bool IsInitialized => isInitialized;

    public event Action<IAnimationState, IAnimationState> OnStateChanged;

    public AdvancedAnimationStateMachine(IAnimationPlayer player, IAnimationEventHandler eventHandler)
    {
        this.player = player;
        this.eventHandler = eventHandler;
    }

    public void Initialize()
    {
        if (isInitialized) return;
        isInitialized = true;
    }

    public void AddState(IAnimationState state)
    {
        states[state.Name] = state;
    }

    public void RemoveState(string stateName)
    {
        states.Remove(stateName);
    }

    public void TransitionTo(string stateName, object transitionData = null)
    {
        if (!states.TryGetValue(stateName, out var nextState))
        {
            Debug.LogError($"State {stateName} not found");
            return;
        }

        if (currentState != null && !currentState.CanTransitionTo(nextState))
        {
            Debug.LogWarning($"Cannot transition from {currentState.Name} to {nextState.Name}");
            return;
        }

        previousState = currentState;
        currentState?.OnExit();

        currentState = nextState;
        currentState.OnEnter(transitionData);

        // 播放动画
        if (currentState.Clip != null)
        {
            player.Play(currentState.Clip, GetFadeTime(currentState), currentState.Layer);
        }
        else
        {
            player.Play(currentState.Name, GetFadeTime(currentState), currentState.Layer);
        }

        OnStateChanged?.Invoke(previousState, currentState);
    }

    private float GetFadeTime(IAnimationState state)
    {
        // 根据优先级动态计算过渡时间
        switch (state.Priority)
        {
            case AnimationPriority.Highest:
            case AnimationPriority.Critical:
                return 0.05f;
            case AnimationPriority.High:
                return 0.1f;
            default:
                return 0.2f;
        }
    }

    public void OnUpdate(float deltaTime)
    {
        currentState?.OnUpdate(deltaTime);
    }

    public void OnLateUpdate(float deltaTime)
    {
    }

    public void OnFixedUpdate(float deltaTime)
    {
    }

    public void Dispose()
    {
        currentState?.OnExit();
        states.Clear();
        isInitialized = false;
    }
}


