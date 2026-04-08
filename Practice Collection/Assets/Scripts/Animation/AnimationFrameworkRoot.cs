using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 动画框架根组件 - 框架入口
/// </summary>
public class AnimationFrameworkRoot : MonoBehaviour
{
    [Header("Core Settings")] [SerializeField]
    private Animator targetAnimator;

    [SerializeField] private ScriptableAnimationDataProvider animationDatabase;

    [Header("Modules")] [SerializeField] private List<AnimationModule> modules = new List<AnimationModule>();

    [Header("Debug")] [SerializeField] private bool enableDebugLogs = false;

    private AnimationServiceContainer container;
    private bool isInitialized;

    public AnimationServiceContainer Container => container;
    public bool IsInitialized => isInitialized;

    private void Awake()
    {
        InitializeFramework();
    }

    private void InitializeFramework()
    {
        if (isInitialized) return;

        // 1. 创建服务容器
        container = new AnimationServiceContainer();

        // 2. 注册核心服务
        RegisterCoreServices();

        // 3. 初始化数据提供者
        InitializeDataProvider();

        // 4. 初始化所有模块
        InitializeModules();

        isInitialized = true;

        if (enableDebugLogs)
            Debug.Log($"Animation Framework initialized on {gameObject.name}");
    }

    private void RegisterCoreServices()
    {
        // 注册动画播放器
        var player = new UnityAnimationPlayer(targetAnimator);
        container.RegisterSingleton<IAnimationPlayer>(player);

        // 注册事件处理器
        var eventHandler = new AnimationEventDispatcher();
        container.RegisterSingleton<IAnimationEventHandler>(eventHandler);

        // 注册状态机
        var stateMachine = new AdvancedAnimationStateMachine(player, eventHandler);
        container.RegisterSingleton<IAnimationStateMachine>(stateMachine);

        // 注册数据提供者
        container.RegisterSingleton<IAnimationDataProvider>(animationDatabase);
    }

    private void InitializeDataProvider()
    {
        var dataProvider = container.Resolve<IAnimationDataProvider>();
        dataProvider.Initialize();

        var eventHandler = container.Resolve<IAnimationEventHandler>();
        eventHandler.Initialize();

        var stateMachine = container.Resolve<IAnimationStateMachine>();
        stateMachine.Initialize();

        var player = container.Resolve<IAnimationPlayer>();
        player.Initialize();
    }

    private void InitializeModules()
    {
        foreach (var module in modules)
        {
            if (module != null)
            {
                module.Initialize(container);
                if (enableDebugLogs)
                    Debug.Log($"Module {module.ModuleName} initialized");
            }
        }
    }

    private void Update()
    {
        if (!isInitialized) return;

        float deltaTime = Time.deltaTime;

        // 更新状态机
        var stateMachine = container.Resolve<IAnimationStateMachine>();
        stateMachine.OnUpdate(deltaTime);

        // 更新所有模块
        foreach (var module in modules)
        {
            if (module != null && module.IsInitialized)
            {
                module.OnUpdate(deltaTime);
            }
        }
    }

    private void OnDestroy()
    {
        if (!isInitialized) return;

        // 清理模块
        foreach (var module in modules)
        {
            module?.Shutdown();
        }

        // 清理服务
        var player = container.Resolve<IAnimationPlayer>();
        player?.Dispose();

        isInitialized = false;

        if (enableDebugLogs)
            Debug.Log($"Animation Framework shutdown on {gameObject.name}");
    }

    /// <summary>
    /// 获取模块
    /// </summary>
    public T GetModule<T>() where T : AnimationModule
    {
        return modules.Find(m => m is T) as T;
    }

    /// <summary>
    /// 获取服务
    /// </summary>
    public T GetService<T>() where T : class
    {
        return container.Resolve<T>();
    }
}