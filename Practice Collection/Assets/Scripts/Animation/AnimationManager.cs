using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    private Animator animator;
    private Dictionary<string, int> animationHashes;
    
    void Awake()
    {
        animator = GetComponentInChildren<Animator>();
        animationHashes = new Dictionary<string, int>();
    }
    
    public void Play(string stateName, float crossFade = 0.1f, int layer = 0)
    {
        if (animator == null) return;
        
        int hash = GetAnimationHash(stateName);
        animator.CrossFade(hash, crossFade, layer);
    }
    
    public void SetFloat(string param, float value)
    {
        if (animator == null) return;
        animator.SetFloat(param, value);
    }
    
    public void SetBool(string param, bool value)
    {
        if (animator == null) return;
        animator.SetBool(param, value);
    }
    
    public void SetTrigger(string param)
    {
        if (animator == null) return;
        animator.SetTrigger(param);
    }
    
    private int GetAnimationHash(string stateName)
    {
        if (!animationHashes.ContainsKey(stateName))
        {
            animationHashes[stateName] = Animator.StringToHash(stateName);
        }
        return animationHashes[stateName];
    }
}
