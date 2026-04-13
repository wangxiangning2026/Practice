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
            character.IsCrouching = !character.IsCrouching;
            character.SetCrouch(!character.IsCrouching);
            character.SetAnimatorBool("IsCrouching", character.IsCrouching);
        }
        // 状态切换逻辑
        else if (character.IsMoving())
        {
            if (Input.GetKey(KeyCode.LeftShift))
                fsm.ChangeState<RunState>();
            else
                fsm.ChangeState<WalkState>();
        }
        else if (Input.GetButtonDown("Jump"))
        {
            fsm.ChangeState<JumpState>();
        }
    }
}