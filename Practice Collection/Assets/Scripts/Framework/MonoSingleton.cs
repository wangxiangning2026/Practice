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
    private static readonly object _lock = new object();
    
    // 标记应用程序是否正在退出，防止在销毁阶段创建新实例
    private static bool _isQuitting = false;

    public static T Instance
    {
        get
        {
            // 1. 如果正在退出，直接返回 null，避免报错或创建孤儿对象
            if (_isQuitting)
            {
                Debug.LogWarning($"[Singleton] 实例 '{typeof(T)}' 正在被销毁，无法创建新实例。");
                return null;
            }

            lock (_lock)
            {
                // 2. 双重检查锁
                if (_instance == null)
                {
                    // 尝试在场景中查找（支持手动拖入场景的情况）
                    _instance = FindObjectOfType<T>();

                    if (_instance == null)
                    {
                        // 如果场景中没有，则动态创建
                        GameObject singletonObject = new GameObject($"[Singleton] {typeof(T).Name}");
                        _instance = singletonObject.AddComponent<T>();
                        
                        // 3. 确保跨场景不销毁
                        DontDestroyOnLoad(singletonObject);
                        
                        Debug.Log($"[Singleton] 创建了新的实例: {typeof(T).Name}");
                    }
                    else
                    {
                        // 如果是场景中找到的，也建议加上 DontDestroyOnLoad
                        // 防止场景切换导致单例意外丢失
                        DontDestroyOnLoad(_instance.gameObject);
                    }
                }
                return _instance;
            }
        }
    }

    // 4. 处理应用程序退出
    protected virtual void OnApplicationQuit()
    {
        _isQuitting = true;
    }

    // 5. 处理对象销毁（例如手动 Destroy 或场景卸载）
    protected virtual void OnDestroy()
    {
        // 只有当销毁的是当前单例本身时，才清空静态引用
        if (_instance == this)
        {
            _instance = null;
            // 注意：这里不重置 _isQuitting，因为 OnApplicationQuit 可能先于 OnDestroy 调用
        }
    }

    // 6. 防止场景中存在多个实例（防御性编程）
    protected virtual void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning($"[Singleton] 检测到重复实例，销毁多余的: {gameObject.name}");
            Destroy(gameObject);
            return;
        }
        
        // 如果通过 Inspector 放置了物体，Awake 会比 Instance 属性访问先执行
        // 所以这里需要同步静态变量
        if (_instance == null)
        {
            _instance = this as T;
            DontDestroyOnLoad(gameObject);
        }
    }
}
