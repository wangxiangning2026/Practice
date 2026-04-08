using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region 核心接口定义

/// <summary>
/// 可初始化接口
/// </summary>
public interface IInitializable
{
    void Initialize();
    bool IsInitialized { get; }
}

/// <summary>
/// 可销毁接口
/// </summary>
public interface IDisposable
{
    void Dispose();
}

/// <summary>
/// 可更新接口
/// </summary>
public interface IUpdatable
{
    void OnUpdate(float deltaTime);
    void OnLateUpdate(float deltaTime);
    void OnFixedUpdate(float deltaTime);
}



/// <summary>
/// 动画状态接口
/// </summary>
public interface IAnimationState
{
    string Name { get; }
    AnimationClip Clip { get; }
    float Duration { get; }
    int Layer { get; }
    AnimationPriority Priority { get; }
    void OnEnter(object transitionData);
    void OnUpdate(float deltaTime);
    void OnExit();
    bool CanTransitionTo(IAnimationState nextState);
}





public delegate void AnimationEventDelegate(AnimationEventContext context);

/// <summary>
/// 动画事件上下文
/// </summary>
public struct AnimationEventContext
{
    public string EventName;
    public GameObject Source;
    public object Parameter;
    public float EventTime;
    public AnimationClip SourceClip;

    public T GetParameter<T>()
    {
        if (Parameter is T)
            return (T)Parameter;
        return default;
    }
}

#endregion

#region 基础实现

/// <summary>
/// 动画状态基类
/// </summary>
public abstract class BaseAnimationState : IAnimationState
{
    public virtual string Name => GetType().Name;
    public virtual AnimationClip Clip { get; protected set; }
    public virtual float Duration => Clip != null ? Clip.length : 0f;
    public virtual int Layer { get; protected set; } = 0;
    public virtual AnimationPriority Priority { get; protected set; } = AnimationPriority.Normal;

    protected IAnimationPlayer player;
    protected IAnimationEventHandler eventHandler;

    public BaseAnimationState(IAnimationPlayer player, IAnimationEventHandler eventHandler)
    {
        this.player = player;
        this.eventHandler = eventHandler;
    }

    public virtual void OnEnter(object transitionData)
    {
    }

    public virtual void OnUpdate(float deltaTime)
    {
    }

    public virtual void OnExit()
    {
    }

    public virtual bool CanTransitionTo(IAnimationState nextState)
    {
        return nextState.Priority >= Priority;
    }
}

/// <summary>
/// 服务容器（IoC容器）
/// </summary>
public class AnimationServiceContainer
{
    private readonly Dictionary<Type, object> services = new Dictionary<Type, object>();
    private readonly Dictionary<Type, Func<object>> factories = new Dictionary<Type, Func<object>>();

    public void RegisterSingleton<T>(T service) where T : class
    {
        var type = typeof(T);
        if (services.ContainsKey(type))
            throw new InvalidOperationException($"Service {type} already registered");
        services[type] = service;
    }

    public void RegisterSingleton<T>(Func<T> factory) where T : class
    {
        var type = typeof(T);
        factories[type] = () => factory();
    }

    public void RegisterTransient<T>(Func<T> factory) where T : class
    {
        var type = typeof(T);
        factories[type] = () => factory();
    }

    public T Resolve<T>() where T : class
    {
        var type = typeof(T);
        if (services.TryGetValue(type, out var service))
            return service as T;

        if (factories.TryGetValue(type, out var factory))
        {
            var instance = factory() as T;
            if (instance is IInitializable initializable && !initializable.IsInitialized)
                initializable.Initialize();
            return instance;
        }

        throw new KeyNotFoundException($"Service {type} not registered");
    }

    public bool TryResolve<T>(out T service) where T : class
    {
        try
        {
            service = Resolve<T>();
            return true;
        }
        catch
        {
            service = null;
            return false;
        }
    }
}

#endregion