
/// <summary>
/// 动画事件处理器接口
/// </summary>
public interface IAnimationEventHandler : IInitializable
{
    void DispatchEvent(string eventName, AnimationEventContext context);
    void RegisterHandler(string eventName, AnimationEventDelegate handler);
    void UnregisterHandler(string eventName, AnimationEventDelegate handler);
    void ClearHandlers();
}
