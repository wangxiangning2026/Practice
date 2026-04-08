using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 脚本化对象数据提供者
/// </summary>
[CreateAssetMenu(fileName = "AnimationDatabase", menuName = "Animation Framework/Animation Database")]
public class ScriptableAnimationDataProvider : ScriptableObject, IAnimationDataProvider
{
    [SerializeField] private List<AnimationConfig> animations = new List<AnimationConfig>();
    [SerializeField] private List<AnimationTagGroup> tagGroups = new List<AnimationTagGroup>();

    private Dictionary<string, AnimationConfig> configMap;
    private Dictionary<string, List<AnimationConfig>> tagMap;
    private bool isInitialized;

    public bool IsInitialized => isInitialized;

    [System.Serializable]
    public class AnimationConfig
    {
        public string name;
        public AnimationClip clip;
        public AnimationPriority priority = AnimationPriority.Normal;
        public int layer = 0;
        public float fadeTime = 0.1f;
        public List<string> tags = new List<string>();
        public List<AnimationEventData> events = new List<AnimationEventData>();
        public bool rootMotion = false;
    }

    [System.Serializable]
    public class AnimationTagGroup
    {
        public string tagName;
        public List<string> animationNames;
    }

    public void Initialize()
    {
        if (isInitialized) return;

        configMap = new Dictionary<string, AnimationConfig>();
        tagMap = new Dictionary<string, List<AnimationConfig>>();

        foreach (var config in animations)
        {
            configMap[config.name] = config;

            foreach (var tag in config.tags)
            {
                if (!tagMap.ContainsKey(tag))
                    tagMap[tag] = new List<AnimationConfig>();
                tagMap[tag].Add(config);
            }
        }

        isInitialized = true;
    }

    public AnimationClip GetClip(string clipName)
    {
        return configMap.TryGetValue(clipName, out var config) ? config.clip : null;
    }

    public AnimationClip[] GetClipsByTag(string tag)
    {
        if (tagMap.TryGetValue(tag, out var configs))
            return configs.Select(c => c.clip).ToArray();
        return Array.Empty<AnimationClip>();
    }

    public float GetClipLength(string clipName)
    {
        var clip = GetClip(clipName);
        return clip != null ? clip.length : 0f;
    }

    public AnimationEventData[] GetClipEvents(string clipName)
    {
        return configMap.TryGetValue(clipName, out var config)
            ? config.events.ToArray()
            : Array.Empty<AnimationEventData>();
    }

    public bool TryGetAnimationConfig(string animationName, out AnimationConfig config)
    {
        return configMap.TryGetValue(animationName, out config);
    }
}