using System.Collections.Generic;

public enum FSMStateType
{
    //人物动作相关
    
}

public class FSM<T> where T : IFSMState
{
    public Dictionary<FSMStateType, T> StateTable{ get; protected set; } //状态表
    public T PrevState{ get; protected set; } //前一个状态
    protected T curState; //当前状态
    public FSM()
    {
        StateTable = new Dictionary<FSMStateType, T>();
        curState = PrevState = default;
    }
    public void AddState(FSMStateType type,T state)
    {
        StateTable.Add(type, state);
    }
    public void SwitchOn(T startState)
    {
        curState = startState;
        curState.Enter();
    }
    public void SwitchOn(FSMStateType startState)
    {
        curState = StateTable[startState];
        curState.Enter();
    }
    public void ChangeState(T nextState)
    {
        PrevState = curState;
        curState.Exit();
        curState = nextState;
        curState.Enter();
    }
    public void ChangeState(FSMStateType nextState)
    {
        PrevState = curState;
        curState.Exit();
        curState = StateTable[nextState];
        curState.Enter();
    }
    public void RevertToPrevState()
    {
        if(PrevState != null) 
        {
            ChangeState(PrevState);
        }
    }
    public void OnUpdate()
    {
        curState.LogicalUpdate();
    }
}