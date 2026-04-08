using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    CharacterStateType StateName { get; }
    void OnEnter();
    void OnUpdate(float deltaTime);
    void OnFixedUpdate(float deltaTime);
    void OnExit();
}

/// <summary>
/// 状态机 - 管理状态切换
/// </summary>
public class StateMachine
{
    private IState currentState;
    private IState previousState;
    private Dictionary<Type, IState> states = new Dictionary<Type, IState>();

    public IState CurrentState => currentState;
    public IState PreviousState => previousState;
    public float StateTime { get; private set; }

    /// <summary>
    /// 添加状态
    /// </summary>
    public void AddState<T>(T state) where T : IState
    {
        states[typeof(T)] = state;
    }

    /// <summary>
    /// 获取状态
    /// </summary>
    public T GetState<T>() where T : IState
    {
        return (T)states[typeof(T)];
    }

    /// <summary>
    /// 切换状态
    /// </summary>
    public void ChangeState<T>() where T : IState
    {
        var newState = GetState<T>();
        if (newState == null)
        {
            Debug.LogError($"状态 {typeof(T)} 不存在");
            return;
        }

        if (currentState != null && currentState.GetType() == typeof(T))
            return;

        // 退出当前状态
        currentState?.OnExit();

        // 切换状态
        previousState = currentState;
        currentState = newState;
        StateTime = 0f;

        // 进入新状态
        currentState.OnEnter();
    }

    /// <summary>
    /// 切换状态（带参数）
    /// </summary>
    public void ChangeState<T>(Action<T> onEnter) where T : IState
    {
        ChangeState<T>();
        onEnter?.Invoke((T)currentState);
    }

    /// <summary>
    /// 更新
    /// </summary>
    public void Update(float deltaTime)
    {
        StateTime += deltaTime;
        currentState?.OnUpdate(deltaTime);
    }

    /// <summary>
    /// 固定更新（物理）
    /// </summary>
    public void FixedUpdate(float deltaTime)
    {
        currentState?.OnFixedUpdate(deltaTime);
    }

    /// <summary>
    /// 强制切换状态（忽略条件）
    /// </summary>
    public void ForceChangeState<T>() where T : IState
    {
        var newState = GetState<T>();
        if (newState == null) return;

        currentState?.OnExit();
        previousState = currentState;
        currentState = newState;
        StateTime = 0f;
        currentState.OnEnter();
    }

    /// <summary>
    /// 可切换条件的状态接口
    /// </summary>
    public interface IConditionalState : IState
    {
        bool CanTransitionTo<T>() where T : IState;
    }
}

public enum CharacterStateType
{
    Idle,
    Walk,
    Run,
    Jump,
    Attack,
    Dead
}