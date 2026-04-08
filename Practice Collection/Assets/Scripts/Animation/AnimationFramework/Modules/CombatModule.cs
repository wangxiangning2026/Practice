using System;
using UnityEngine;

/// <summary>
/// 战斗动画模块
/// </summary>
public class CombatModule : AnimationModule
{
    [Header("Combat Settings")] [SerializeField]
    private float attackCooldown = 0.5f;

    [SerializeField] private int maxComboCount = 3;

    private IAnimationStateMachine stateMachine;
    private IAnimationEventHandler eventHandler;

    private int currentCombo;
    private float lastAttackTime;
    private bool isAttacking;

    public event Action<int> OnAttackExecuted;
    public event Action OnComboFinished;

    public override void Initialize(AnimationServiceContainer container)
    {
        base.Initialize(container);
        stateMachine = container.Resolve<IAnimationStateMachine>();
        eventHandler = container.Resolve<IAnimationEventHandler>();

        SetupCombatStates();
        RegisterCombatEvents();
    }

    private void SetupCombatStates()
    {
        var builder = new AnimationStateBuilder(
            container.Resolve<IAnimationPlayer>(),
            container.Resolve<IAnimationEventHandler>()
        );

        // 设置攻击状态
        for (int i = 0; i < maxComboCount; i++)
        {
            int comboIndex = i;
            var attackState = builder
                .WithName($"Attack{comboIndex + 1}")
                .WithPriority(AnimationPriority.High)
                .OnEnter(_ => OnAttackStart(comboIndex))
                .OnExit(() => OnAttackEnd())
                .Build();
            stateMachine.AddState(attackState);
        }
    }

    private void RegisterCombatEvents()
    {
        eventHandler.RegisterHandler("OnAttackHit", OnAttackHit);
        eventHandler.RegisterHandler("OnAttackEnd", OnAttackEndEvent);
    }

    public bool ExecuteAttack()
    {
        if (isAttacking)
        {
            // 连击逻辑
            if (Time.time - lastAttackTime < attackCooldown && currentCombo < maxComboCount - 1)
            {
                currentCombo++;
                return ExecuteCurrentCombo();
            }

            return false;
        }

        currentCombo = 0;
        return ExecuteCurrentCombo();
    }

    private bool ExecuteCurrentCombo()
    {
        string attackStateName = $"Attack{currentCombo + 1}";
        stateMachine.TransitionTo(attackStateName);
        lastAttackTime = Time.time;
        return true;
    }

    private void OnAttackStart(int comboIndex)
    {
        isAttacking = true;
        OnAttackExecuted?.Invoke(comboIndex);
    }

    private void OnAttackEnd()
    {
        isAttacking = false;
        if (currentCombo >= maxComboCount - 1)
        {
            currentCombo = 0;
            OnComboFinished?.Invoke();
        }
    }

    private void OnAttackHit(AnimationEventContext context)
    {
        // 处理攻击命中
        Debug.Log($"Attack hit at time: {context.EventTime}");
    }

    private void OnAttackEndEvent(AnimationEventContext context)
    {
        OnAttackEnd();
    }

    public override void OnUpdate(float deltaTime)
    {
        // 更新冷却时间等
    }

    public override void Shutdown()
    {
        base.Shutdown();
        eventHandler.UnregisterHandler("OnAttackHit", OnAttackHit);
        eventHandler.UnregisterHandler("OnAttackEnd", OnAttackEndEvent);
    }
}
