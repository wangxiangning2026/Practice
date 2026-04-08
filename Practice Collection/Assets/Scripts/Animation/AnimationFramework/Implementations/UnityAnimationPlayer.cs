using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unity动画播放器实现
/// </summary>
public class UnityAnimationPlayer : IAnimationPlayer
{
    private readonly Animator animator;
    private readonly Dictionary<string, int> stateHashes;
    private bool isInitialized;
    private bool isPaused;

    public bool IsInitialized => isInitialized;
    public bool IsPlaying => !isPaused && animator != null && animator.enabled;

    public UnityAnimationPlayer(Animator animator)
    {
        this.animator = animator;
        this.stateHashes = new Dictionary<string, int>();
    }

    public void Initialize()
    {
        if (isInitialized) return;
        if (animator == null) throw new ArgumentNullException(nameof(animator));

        // 预计算所有状态的哈希值
        if (animator.runtimeAnimatorController != null)
        {
            foreach (var clip in animator.runtimeAnimatorController.animationClips)
            {
                stateHashes[clip.name] = Animator.StringToHash(clip.name);
            }
        }

        isInitialized = true;
    }

    public void Play(string stateName, float crossFade = 0.1f, int layer = 0)
    {
        if (!isInitialized) throw new InvalidOperationException("Player not initialized");
        if (isPaused) return;

        int hash = GetStateHash(stateName);
        animator.CrossFadeInFixedTime(hash, crossFade, layer);
    }

    public void Play(AnimationClip clip, float crossFade = 0.1f, int layer = 0)
    {
        if (!isInitialized) return;
        if (isPaused) return;

        // 使用动画层和Playable API实现直接播放
        animator.CrossFadeInFixedTime(clip.name, crossFade, layer);
    }

    public void Stop(int layer = 0)
    {
        if (!isInitialized) return;
        animator.StopPlayback();
    }

    public void Pause()
    {
        isPaused = true;
        animator.speed = 0f;
    }

    public void Resume()
    {
        isPaused = false;
        animator.speed = 1f;
    }

    public bool IsPlayingState(string stateName)
    {
        if (!isInitialized) return false;
        var stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.IsName(stateName);
    }

    public float GetCurrentStateNormalizedTime(int layer = 0)
    {
        if (!isInitialized) return 0f;
        var stateInfo = animator.GetCurrentAnimatorStateInfo(layer);
        return stateInfo.normalizedTime;
    }

    private int GetStateHash(string stateName)
    {
        if (stateHashes.TryGetValue(stateName, out int hash))
            return hash;

        hash = Animator.StringToHash(stateName);
        stateHashes[stateName] = hash;
        return hash;
    }

    public void Dispose()
    {
        isInitialized = false;
        stateHashes.Clear();
    }
}