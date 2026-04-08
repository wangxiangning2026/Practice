using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkState : FMSStateBase
{
    private PlayerController character;
        
    public WalkState(FSMController controller, PlayerController charCtrl) : base(controller)
    {
        character = charCtrl;
    }
        
    public override void Enter()
    {
        character.PlayAnimation("Walk");
        character.SetAnimatorFloat("Speed", 0.5f);
    }

    public override void LogicalUpdate()
    {
        float currentSpead = character.walkSpeed;
        // 状态切换
        if (!character.isMoving)
        {
            fsm.ChangeState<IdleState>();
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            fsm.ChangeState<RunState>();
            currentSpead = character.runSpeed;
        }
        // else if (Input.GetButtonDown("Jump") && character.IsGrounded)
        // {
        //     fsm.ChangeState<JumpState>();
        // }
        // 移动
        character.HandleMovement(currentSpead);
    }

    public override void LogicalFixedUpdate()
    {
        // 可在此处理物理相关逻辑
    }
        
    public override void Exit()
    {
        character.SetAnimatorFloat("Speed", 0);
    }
}
