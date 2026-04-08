
public class FMSStateBase : IFSMState
{
    protected FSMController fsm;
    public virtual string StateName => GetType().Name;

    public FMSStateBase(FSMController controller)
    {
        fsm = controller;
    }

    public virtual void Enter() { }
    public virtual void LogicalUpdate() { }
    public virtual void LogicalFixedUpdate() { }
    public virtual void Exit() { }
}
