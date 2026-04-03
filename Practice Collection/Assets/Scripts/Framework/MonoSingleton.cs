using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 线程安全的 MonoSingleton 基类
/// 适用于 GameManager, AudioManager 等需要跨场景持久化且可能被多线程访问的管理器
/// </summary>
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;
    private static readonly object _lock = new object(); // 1. 线程锁

    public static T Instance
    {
        get
        {
            // 2. 双重检查锁模式，避免频繁查找和线程冲突
            if (_instance == null)
            {
                lock (_lock)
                {
                    if (_instance == null)
                    {
                        // 尝试在场景中查找（防止手动拖入场景的情况）
                        _instance = FindObjectOfType<T>();
                        
                        // 如果场景中也没有，则动态创建
                        if (_instance == null)
                        {
                            GameObject singletonObject = new GameObject(typeof(T).Name + " (Singleton)");
                            _instance = singletonObject.AddComponent<T>();
                            // 3. 关键：确保跨场景不销毁
                            DontDestroyOnLoad(singletonObject); 
                        }
                        else
                        {
                            // 如果是场景中找到的，也建议加上 DontDestroyOnLoad
                            DontDestroyOnLoad(_instance.gameObject);
                        }
                    }
                }
            }
            return _instance;
        }
    }

    protected virtual void Awake()
    {
        // 4. 防止场景中存在多个实例（例如开发时误拖了两个）
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        
        // 确保单例在 Awake 阶段就被赋值，防止 Instance 属性被访问前的空值
        _instance = this as T;
        DontDestroyOnLoad(gameObject);
    }

    // 5. 可选：程序退出时清理引用，防止编辑器下的脏数据
    protected virtual void OnDestroy()
    {
        if (_instance == this)
        {
            _instance = null;
        }
    }
}
