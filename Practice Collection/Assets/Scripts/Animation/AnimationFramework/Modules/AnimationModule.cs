using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 模块基类
/// </summary>
public abstract class AnimationModule : MonoBehaviour
{
    [SerializeField] protected string moduleName;
    protected AnimationServiceContainer container;
    protected bool isInitialized;

    public string ModuleName => moduleName;
    public bool IsInitialized => isInitialized;

    public virtual void Initialize(AnimationServiceContainer container)
    {
        this.container = container;
        isInitialized = true;
    }

    public virtual void Shutdown()
    {
        isInitialized = false;
    }

    public abstract void OnUpdate(float deltaTime);
}