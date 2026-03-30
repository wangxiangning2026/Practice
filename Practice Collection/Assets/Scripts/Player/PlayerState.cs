using UnityEngine;

public class PlayerState : IFSMState
{
    protected readonly int animHash;
    protected PlayerController agent;

    public PlayerState(PlayerController agent, string animName)
    {
        this.agent = agent;
        animHash = Animator.StringToHash(animName);
    }

    public virtual void Enter()
    {
        agent.animator.CrossFade(animHash, 0.1f);
    }

    public virtual void Exit()
    {
        ;
    }

    public virtual void LogicalUpdate()
    {
        ;
    }
}