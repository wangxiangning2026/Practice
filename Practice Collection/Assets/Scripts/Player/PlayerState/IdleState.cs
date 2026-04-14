using UnityEngine;

public class IdleState : FMSStateBase
{
    private PlayerController character;

    public IdleState(FSMController controller, PlayerController charCtrl) : base(controller)
    {
        character = charCtrl;
    }

    public override void Enter()
    {
        character.SetAnimatorFloat("targetSpeed", 0f);
    }

    public override void LogicalUpdate()
    {
        // 下蹲切换
        if (Input.GetKeyDown(KeyCode.C))
        {
            character.SetCrouch(!character.IsCrouching);
            character.PlayAnimation(character.IsCrouching ? "CrouchIdle" : "Idle");
            return;
        }
        
        // 状态切换逻辑
        if (character.IsMoving())
        {
            if (Input.GetKey(KeyCode.LeftShift))
                fsm.ChangeState<RunState>();
            else
                fsm.ChangeState<WalkState>();
        }
        // else if (Input.GetButtonDown("Jump") && character.IsGrounded)
        // {
        //     fsm.ChangeState<JumpState>();
        // }
    }
}