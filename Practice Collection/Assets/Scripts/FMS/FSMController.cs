using System;
using System.Collections.Generic;
using UnityEngine;

public class FSMController : MonoBehaviour
{
    [Header("FSM Settings")]
    
    [SerializeField] private bool enableDebugLog = false;
    [SerializeField] private bool enableGizmos = true;
    
    private Dictionary<Type, FMSStateBase> states = new Dictionary<Type, FMSStateBase>();
    private FMSStateBase currentState;
    private FMSStateBase previousState;
    private Type defaultStateType;

    // 公开属性
    public FMSStateBase CurrentState => currentState;
    public FMSStateBase PreviousState => previousState;
    public float StateTime { get; private set; } // 当前状态持续时间

    private void Update()
    {
        if (currentState != null)
        {
            StateTime += Time.deltaTime;
            currentState.LogicalUpdate();
        }
    }

    private void FixedUpdate()
    {
        currentState?.LogicalFixedUpdate();
    }

    /// <summary>
    /// 注册状态
    /// </summary>
    public void RegisterState(FMSStateBase state)
    {
        Type stateType = state.GetType();
        if (!states.ContainsKey(stateType))
        {
            states.Add(stateType, state);
        }
    }

    /// <summary>
    /// 批量注册状态
    /// </summary>
    public void RegisterStates(params FMSStateBase[] stateList)
    {
        foreach (var state in stateList)
            RegisterState(state);
    }

    /// <summary>
    /// 设置默认状态
    /// </summary>
    public void SetDefaultState<T>() where T : FMSStateBase
    {
        defaultStateType = typeof(T);
    }

    /// <summary>
    /// 初始化状态机
    /// </summary>
    public void Initialize()
    {
        if (defaultStateType != null)
            ChangeState(defaultStateType);
    }

    /// <summary>
    /// 切换状态
    /// </summary>
    public void ChangeState<T>() where T : FMSStateBase
    {
        ChangeState(typeof(T));
    }

    /// <summary>
    /// 切换状态（通过类型）
    /// </summary>
    public void ChangeState(Type stateType)
    {
        if (!states.ContainsKey(stateType))
        {
            Debug.LogError($"[FSM] 状态 {stateType.Name} 未注册！");
            return;
        }

        // 如果已经是当前状态，可以选择是否重置
        if (currentState != null && currentState.GetType() == stateType)
        {
            if (enableDebugLog)
                Debug.Log($"[FSM] 已是状态 {currentState.StateName}，忽略切换");
            return;
        }

        // 退出当前状态
        if (currentState != null)
        {
            if (enableDebugLog)
                Debug.Log($"[FSM] 退出状态: {currentState.StateName}");
            currentState.Exit();
        }

        // 记录前一个状态
        previousState = currentState;

        // 切换到新状态
        currentState = states[stateType];
        StateTime = 0f;

        if (enableDebugLog)
            Debug.Log($"[FSM] 进入状态: {currentState.StateName}");

        currentState.Enter();
    }

    /// <summary>
    /// 切换回上一个状态
    /// </summary>
    public void ChangeToPreviousState()
    {
        if (previousState != null)
            ChangeState(previousState.GetType());
    }

    /// <summary>
    /// 获取指定状态
    /// </summary>
    public T GetState<T>() where T : FMSStateBase
    {
        Type stateType = typeof(T);
        if (states.ContainsKey(stateType))
            return (T)states[stateType];
        return default(T);
    }

    /// <summary>
    /// 判断是否处于某状态
    /// </summary>
    public bool IsInState<T>() where T : FMSStateBase
    {
        return currentState != null && currentState.GetType() == typeof(T);
    }

    /// <summary>
    /// 判断是否处于某状态（支持继承）
    /// </summary>
    public bool IsInState<T>(bool includeInherited) where T : FMSStateBase
    {
        if (currentState == null) return false;
        if (includeInherited)
            return currentState is T;
        return currentState.GetType() == typeof(T);
    }
}