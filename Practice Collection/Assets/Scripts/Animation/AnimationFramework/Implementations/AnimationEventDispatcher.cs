using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 动画事件分发器
/// </summary>
public class AnimationEventDispatcher : IAnimationEventHandler
{
    private readonly Dictionary<string, List<AnimationEventDelegate>> handlers = 
        new Dictionary<string, List<AnimationEventDelegate>>();
    private bool isInitialized;
        
    public bool IsInitialized => isInitialized;
        
    public void Initialize()
    {
        isInitialized = true;
    }
        
    public void DispatchEvent(string eventName, AnimationEventContext context)
    {
        if (!isInitialized) return;
            
        if (handlers.TryGetValue(eventName, out var eventHandlers))
        {
            foreach (var handler in eventHandlers)
            {
                try
                {
                    handler.Invoke(context);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error in animation event handler: {e}");
                }
            }
        }
    }
        
    public void RegisterHandler(string eventName, AnimationEventDelegate handler)
    {
        if (!handlers.ContainsKey(eventName))
            handlers[eventName] = new List<AnimationEventDelegate>();
        handlers[eventName].Add(handler);
    }
        
    public void UnregisterHandler(string eventName, AnimationEventDelegate handler)
    {
        if (handlers.TryGetValue(eventName, out var eventHandlers))
            eventHandlers.Remove(handler);
    }
        
    public void ClearHandlers()
    {
        handlers.Clear();
    }
}
