using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ResourceManager : MonoSingleton<ResourceManager>
{
    // 资源缓存（引用计数管理）
    private Dictionary<string, AssetReference> assetRefCache = new Dictionary<string, AssetReference>();
    private Dictionary<string, int> refCounts = new Dictionary<string, int>();
    private Dictionary<string, GameObject> instantiatedCache = new Dictionary<string, GameObject>();

    /// <summary>
    /// 异步加载GameObject预制体（带缓存和引用计数）
    /// </summary>
    public AsyncOperationHandle<GameObject> LoadAssetAsync(string key, Action<GameObject> onComplete)
    {
        // 检查是否已加载且未释放
        if (assetRefCache.TryGetValue(key, out AssetReference assetRef))
        {
            // 增加引用计数
            if (!refCounts.ContainsKey(key))
                refCounts[key] = 0;
            refCounts[key]++;

            // 如果已实例化过，直接返回实例
            if (instantiatedCache.TryGetValue(key, out GameObject cachedInstance) && cachedInstance != null)
            {
                onComplete?.Invoke(cachedInstance);
                return default;
            }

            // 否则重新加载
            return LoadAndInstantiate(key, onComplete);
        }

        // 首次加载
        return LoadAndInstantiate(key, onComplete);
    }

    private AsyncOperationHandle<GameObject> LoadAndInstantiate(string key, Action<GameObject> onComplete)
    {
        var handle = Addressables.LoadAssetAsync<GameObject>(key);
        handle.Completed += (op) =>
        {
            if (op.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject prefab = op.Result;
                GameObject instance = Instantiate(prefab);
                instance.name = prefab.name;

                // 缓存
                if (!assetRefCache.ContainsKey(key))
                {
                    assetRefCache[key] = new AssetReference(key);
                    refCounts[key] = 1;
                }
                else
                {
                    refCounts[key]++;
                }

                instantiatedCache[key] = instance;

                onComplete?.Invoke(instance);
            }
            else
            {
                Debug.LogError($"[AddressableResourceManager] 加载失败: {key}");
                onComplete?.Invoke(null);
            }
        };
        return handle;
    }

    /// <summary>
    /// 异步加载
    /// </summary>
    private IEnumerator LoadSceneAsync(string sceneFullPath, float startPercent, Action success, Action<bool> finish)
    {
        yield return null;
    }

    /// <summary>
    /// 释放资源（减少引用计数）
    /// </summary>
    public void ReleaseAsset(string key)
    {
        if (!refCounts.ContainsKey(key)) return;

        refCounts[key]--;
        if (refCounts[key] <= 0)
        {
            // 引用计数为0，真正释放
            if (instantiatedCache.TryGetValue(key, out GameObject instance))
            {
                Destroy(instance);
                instantiatedCache.Remove(key);
            }

            if (assetRefCache.TryGetValue(key, out AssetReference assetRef))
            {
                Addressables.Release(assetRef);
                assetRefCache.Remove(key);
            }

            refCounts.Remove(key);
        }
    }

    /// <summary>
    /// 清理所有缓存
    /// </summary>
    public void ClearAllCache()
    {
        foreach (var key in new List<string>(assetRefCache.Keys))
        {
            ReleaseAsset(key);
        }

        Addressables.ClearDependencyCacheAsync("default");
    }

    /// <summary>
    /// 预加载UI资源（游戏启动时调用）
    /// </summary>
    public void PreloadUI(List<string> keys, Action onComplete)
    {
        int total = keys.Count;
        int completed = 0;

        foreach (string key in keys)
        {
            Addressables.LoadAssetAsync<GameObject>(key).Completed += (op) =>
            {
                completed++;
                if (op.Status == AsyncOperationStatus.Succeeded)
                {
                    // 预加载完成，不实例化，只缓存AssetReference
                    if (!assetRefCache.ContainsKey(key))
                    {
                        assetRefCache[key] = new AssetReference(key);
                        refCounts[key] = 0; // 引用计数为0，后续实例化时再增加
                    }
                }

                if (completed >= total)
                {
                    onComplete?.Invoke();
                }
            };
        }
    }
}