using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 移动动画模块
/// </summary>
public class MovementModule : AnimationModule
{
    [Header("Movement Settings")] [SerializeField]
    private float walkSpeed = 2f;

    [SerializeField] private float runSpeed = 6f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float turnSmoothTime = 0.1f;

    private IAnimationStateMachine stateMachine;
    private IAnimationPlayer player;
    private CharacterController controller;

    private float currentSpeed;
    private Vector3 moveDirection;
    private float turnSmoothVelocity;

    public override void Initialize(AnimationServiceContainer container)
    {
        base.Initialize(container);
        stateMachine = container.Resolve<IAnimationStateMachine>();
        player = container.Resolve<IAnimationPlayer>();
        controller = GetComponent<CharacterController>();

        SetupMovementStates();
    }

    private void SetupMovementStates()
    {
        var builder = new AnimationStateBuilder(
            container.Resolve<IAnimationPlayer>(),
            container.Resolve<IAnimationEventHandler>()
        );

        // 空闲状态
        var idleState = builder
            .WithName("Idle")
            .OnEnter(_ => currentSpeed = 0)
            .Build();
        stateMachine.AddState(idleState);

        // 行走状态
        var walkState = builder
            .WithName("Walk")
            .OnEnter(_ => currentSpeed = walkSpeed)
            .Build();
        stateMachine.AddState(walkState);

        // 跑步状态
        var runState = builder
            .WithName("Run")
            .OnEnter(_ => currentSpeed = runSpeed)
            .Build();
        stateMachine.AddState(runState);
    }

    public void UpdateMovement(Vector3 direction, bool sprint = false)
    {
        if (direction.magnitude < 0.1f)
        {
            if (stateMachine.CurrentState?.Name != "Idle")
                stateMachine.TransitionTo("Idle");
            return;
        }

        string targetState = sprint ? "Run" : "Walk";
        if (stateMachine.CurrentState?.Name != targetState)
            stateMachine.TransitionTo(targetState);

        moveDirection = direction;
        UpdateRotation(direction);
        UpdateMovementPhysics();
    }

    private void UpdateRotation(Vector3 direction)
    {
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle,
            ref turnSmoothVelocity, turnSmoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    private void UpdateMovementPhysics()
    {
        if (controller != null && currentSpeed > 0)
        {
            Vector3 move = transform.forward * currentSpeed * Time.deltaTime;
            controller.Move(move);
        }
    }

    public override void OnUpdate(float deltaTime)
    {
        // 更新移动相关逻辑
    }
}