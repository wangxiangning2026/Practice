using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : FMSStateBase
{
    private PlayerController character;
    
    private float gravity = -30f;
    private float jumpForce = 2f;
    
    private Vector3 velocity;
    
    private bool hasJumped = false;
        
    public JumpState(FSMController controller, PlayerController charCtrl) : base(controller)
    {
        character = charCtrl;
    }
        
    public override void Enter()
    {
        character.TriggerAnimator("IsJump");
        
        // 应用跳跃速度
        velocity = character.GetVelocity();
        velocity.y = Mathf.Sqrt(jumpForce * -2f * gravity);
        character.SetVelocity(velocity);
        
        hasJumped = true;
    }

    public override void LogicalUpdate()
    {
        
    }

    public override void LogicalFixedUpdate()
    {
        // 应用重力和移动
        velocity = character.GetVelocity();
        
        // 应用重力
        if (!character.IsGrounded() || velocity.y > 0)
        {
            velocity.y += gravity * Time.fixedDeltaTime;
        }
        else if (character.IsGrounded() && velocity.y < 0)
        {
            velocity.y = -2f;  // 贴地保持
        }
        
        character.SetVelocity(velocity);
        character.MoveCharacter(velocity * Time.fixedDeltaTime);
    }

    public override void Exit()
    {
        character.TriggerAnimator("IsFall");
        hasJumped = false;
    }
    
}
