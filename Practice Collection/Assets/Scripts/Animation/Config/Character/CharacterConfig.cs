using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterConfig", menuName = "FSM/Character Configuration")]
public class CharacterConfig : ScriptableObject
{
    public GameObject prefab;
    
    [Header("基础属性")]
    public string characterName = "New Character";
    public float walkSpeed = 2f;
    public float runSpeed = 5f;
    public float jumpForce = 5f;
        
    [Header("状态特定配置")]
    public StateConfig[] stateConfigs;
        
    [Header("可用状态列表")]
    public List<CharacterStateType> availableStates = new List<CharacterStateType>();
        
    [Header("动画参数映射")]
    public AnimatorParameterConfig animatorParams;
        
    /// <summary>
    /// 获取特定状态的配置
    /// </summary>
    public T GetStateConfig<T>(CharacterStateType stateType) where T : StateConfig
    {
        foreach (var config in stateConfigs)
        {
            if (config.stateType == stateType && config is T)
                return config as T;
        }
        return null;
    }
    
    /// <summary>
    /// 验证配置
    /// </summary>
    public bool Validate()
    {
        if (string.IsNullOrEmpty(characterName))
        {
            Debug.LogError($"配置 {name} 缺少角色名称");
            return false;
        }
            
        if (prefab == null)
        {
            Debug.LogError($"配置 {characterName} 缺少预制体");
            return false;
        }
            
        return true;
    }
}

/// <summary>
/// 状态配置基类
/// </summary>
[System.Serializable]
public abstract class StateConfig
{
    public CharacterStateType stateType;
    public string animationName;
    public float animationCrossFade = 0.1f;
    public bool canTransitionToSelf = false;
}
    
/// <summary>
/// 移动状态配置
/// </summary>
[System.Serializable]
public class MoveStateConfig : StateConfig
{
    public float speedMultiplier = 1f;
    public float acceleration = 10f;
    public float deceleration = 10f;
}
    
/// <summary>
/// 跳跃状态配置
/// </summary>
[System.Serializable]
public class JumpStateConfig : StateConfig
{
    public float jumpForce = 5f;
    public float airControl = 0.5f;
    public int maxAirJumps = 0;
}
    
/// <summary>
/// 攻击状态配置
/// </summary>
[System.Serializable]
public class AttackStateConfig : StateConfig
{
    public float damage = 10f;
    public float attackDuration = 0.5f;
    public float comboWindow = 0.3f;
    public int comboSteps = 3;
}
    
/// <summary>
/// 动画参数配置
/// </summary>
[System.Serializable]
public class AnimatorParameterConfig
{
    public string speedParam = "Speed";
    public string isGroundedParam = "IsGrounded";
    public string jumpParam = "Jump";
    public string attackParam = "Attack";
    public string hurtParam = "Hurt";
    public string dieParam = "Die";
    public string comboIndexParam = "ComboIndex";
}
