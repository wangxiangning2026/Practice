using UnityEngine;

/// <summary>
/// 动画数据提供者接口
/// </summary>
public interface IAnimationDataProvider : IInitializable
{
    AnimationClip GetClip(string clipName);
    AnimationClip[] GetClipsByTag(string tag);
    float GetClipLength(string clipName);
    AnimationEventData[] GetClipEvents(string clipName);
    bool TryGetAnimationConfig(string animationName, out ScriptableAnimationDataProvider.AnimationConfig config);
}
