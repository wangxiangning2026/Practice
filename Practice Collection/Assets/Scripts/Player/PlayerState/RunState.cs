using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RunState : FMSStateBase
{
    private PlayerController character;
        
    public RunState(FSMController controller, PlayerController charCtrl) : base(controller)
    {
        character = charCtrl;
    }
        
    public override void Enter()
    {
        character.PlayAnimation("Run");
        character.SetAnimatorFloat("Speed", 1f);
    }

    public override void LogicalFixedUpdate()
    {
        character.HandleMovement(character.runSpeed);
            
        if (!character.isMoving)
        {
            fsm.ChangeState<IdleState>();
        }
        else if (!Input.GetKey(KeyCode.LeftShift))
        {
            fsm.ChangeState<WalkState>();
        }
        // else if (Input.GetButtonDown("Jump") && character.IsGrounded)
        // {
        //     fsm.ChangeState<JumpState>();
        // }
    }
}
