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
        character.SetAnimatorFloat("targetSpeed", character.walkSpeed);
    }

    public override void LogicalUpdate()
    {
        // 状态切换
        if (!character.IsMoving())
        {
            fsm.ChangeState<IdleState>();
        }
        else if (Input.GetKey(KeyCode.LeftShift))
        {
            fsm.ChangeState<RunState>();
        }
        // else if (Input.GetButtonDown("Jump") && character.IsGrounded)
        // {
        //     fsm.ChangeState<JumpState>();
        // }
       
    }

    public override void LogicalFixedUpdate()
    {
        // 移动
        character.HandleMovement(character.walkSpeed);
    }
        
    public override void Exit()
    {
        character.SetAnimatorFloat("Speed", 0);
    }
}
