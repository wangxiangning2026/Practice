using UnityEngine;

/// <summary>
/// 动画播放器接口
/// </summary>
public interface IAnimationPlayer : IInitializable, IDisposable
{
    void Play(string stateName, float crossFade = 0.1f, int layer = 0);
    void Play(AnimationClip clip, float crossFade = 0.1f, int layer = 0);
    void Stop(int layer = 0);
    void Pause();
    void Resume();
    bool IsPlaying { get; }
    bool IsPlayingState(string stateName);
    float GetCurrentStateNormalizedTime(int layer = 0);
}
