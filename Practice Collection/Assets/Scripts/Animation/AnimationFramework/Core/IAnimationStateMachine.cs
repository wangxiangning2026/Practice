using System;

/// <summary>
/// 动画状态机接口
/// </summary>
public interface IAnimationStateMachine : IInitializable, IDisposable, IUpdatable
{
    void AddState(IAnimationState state);
    void RemoveState(string stateName);
    void TransitionTo(string stateName, object transitionData = null);
    IAnimationState CurrentState { get; }
    IAnimationState PreviousState { get; }
    event Action<IAnimationState, IAnimationState> OnStateChanged;
}
