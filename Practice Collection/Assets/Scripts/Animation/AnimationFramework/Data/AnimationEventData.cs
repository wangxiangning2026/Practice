using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 动画事件数据配置
/// </summary>
[System.Serializable]
public class AnimationEventData
{
    [Header("基础信息")] public string eventName; // 事件名称
    public float eventTime; // 触发时间（秒）

    [Header("参数")] public string stringParameter; // 字符串参数
    public int intParameter; // 整型参数
    public float floatParameter; // 浮点参数
    public Vector3 vectorParameter; // 向量参数
    public bool boolParameter; // 布尔参数

    [Header("高级设置")] public EventTriggerType triggerType = EventTriggerType.Normal;
    public int repeatCount = 1; // 重复次数（-1为无限）
    public float repeatInterval = 0f; // 重复间隔

    public enum EventTriggerType
    {
        Normal, // 普通触发
        Once, // 只触发一次
        Loop, // 循环触发
        Random // 随机时间触发
    }

    public AnimationEventData()
    {
        eventName = "NewEvent";
        eventTime = 0f;
        stringParameter = "";
        intParameter = 0;
        floatParameter = 0f;
        vectorParameter = Vector3.zero;
        boolParameter = false;
        triggerType = EventTriggerType.Normal;
        repeatCount = 1;
        repeatInterval = 0f;
    }

    /// <summary>
    /// 获取参数对象（用于事件传递）
    /// </summary>
    public object GetParameterObject()
    {
        if (!string.IsNullOrEmpty(stringParameter))
            return stringParameter;
        if (intParameter != 0)
            return intParameter;
        if (floatParameter != 0f)
            return floatParameter;
        if (vectorParameter != Vector3.zero)
            return vectorParameter;
        return boolParameter;
    }
}