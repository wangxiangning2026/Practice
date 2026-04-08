using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConfigManager : MonoSingleton<LoadingManager>
{
    [Header("配置路径")]
    [SerializeField] private string characterConfigPath = "Configs/Characters";
    
    // 配置缓存
    private Dictionary<string, CharacterConfig> characterConfigs = new Dictionary<string, CharacterConfig>();
    
    private void Awake()
    {
        LoadAllConfigs();
    }
    
    /// <summary>
    /// 加载所有配置
    /// </summary>
    private void LoadAllConfigs()
    {
        CharacterConfig[] configs = Resources.LoadAll<CharacterConfig>("Configs");
            
        foreach (var config in configs)
        {
            if (config.Validate())
            {
                characterConfigs[config.characterName] = config;
                Debug.Log($"加载角色配置: {config.characterName}");
            }
        }
    }
        
    /// <summary>
    /// 获取配置
    /// </summary>
    public CharacterConfig GetConfig(string characterName)
    {
        if (characterConfigs.TryGetValue(characterName, out var config))
            return config;
            
        Debug.LogError($"找不到角色配置: {characterName}");
        return null;
    }
}
