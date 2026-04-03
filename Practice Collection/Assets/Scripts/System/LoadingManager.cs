using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class LoadingManager : MonoSingleton<LoadingManager>
{
    #region 加载场景

    private Dictionary<string, AsyncOperationHandle<SceneInstance>> loadedSceneHandles = new();

    /// <summary> 
    /// 异步加载指定场景 
    /// </summary> 
    /// <param name="sceneFullPath">场景完整路径</param> 
    /// <param name="startPercent">加载场景的初始进度百分比</param> 
    /// <param name="finish">场景完全加载并激活后的回调</param> 
    /// <param name="success">场景资源加载完成后的回调</param> 
    /// <param name="fail">场景加载失败时的回调</param> 
    /// <returns>协程迭代器</returns> 
    public IEnumerator LoadSceneAsync(string sceneFullPath, float startPercent = 0f, Action finish = null,
        Action success = null, Action fail = null)
    {
        if (loadedSceneHandles.ContainsKey(sceneFullPath))
        {
            Debug.Log($"重复加载场景：{sceneFullPath}"); 
            finish?.Invoke();
            yield break;
        }

        // 设置超时保护，防止异常情况下协程无法结束 
        //Invoke(nameof(DelayClearLoadingCoroutine), 1f); 

        // 显示加载界面 
        //Show(startPercent); 
        //DebugLog.LogByTag(TAG, $"开始加载场景: {sceneFullPath}"); 
        //RefreshHint("正在加载场景资源..."); 

        // 等待一帧，确保UI更新 
        yield return null;

        var isExist = false;
        
        //LoadResourceLocationsAsync 是 Addressables 系统中用于获取资源位置信息的方法。
        //它返回 IResourceLocation 对象的列表，这些对象包含了加载资源所需的所有元数据
        //（如 AssetBundle 路径、依赖项等），但不会实际加载资源内容
        
        // 1. 开始异步操作
        var locationHandle = Addressables.LoadResourceLocationsAsync(sceneFullPath);
        // 2. 暂停协程，等待异步操作完成
        yield return locationHandle;
        
        if (locationHandle is { Status: AsyncOperationStatus.Succeeded, Result: { Count: > 0 } })
        {
            isExist = true;
        }
        
        Addressables.Release(locationHandle);
        
        if (!isExist)
        {
            fail?.Invoke();
            //Hide(); 
            Debug.LogError($"未找到场景资源：{sceneFullPath}");
            yield break;
        }

        // 使用Addressables异步加载场景（附加模式，不立即激活） 
        var handle = Addressables.LoadSceneAsync(sceneFullPath, LoadSceneMode.Additive, false);
        Debug.Log("开始异步加载场景资源"); 

        var curProgress = 0f;
        // 循环检查加载状态直到完成 
        while (handle is { IsDone: false })     //while (!handle.IsDone)
        {
            //Addressables 中用于获取下载进度和状态的方法，特别适用于需要显示加载进度的场景。
            var downloadStatus = handle.GetDownloadStatus();
            // 如果需要下载资源 
            if (downloadStatus.TotalBytes > 0)
            {
                // 资源加载内部可能是分阶段进行的，会出现 Percent 回退，这里限制只允许更大值 
                curProgress = Mathf.Max(curProgress, downloadStatus.Percent);
                //DebugLog.LogByTag(TAG, $"场景加载进度: {handle.PercentComplete:P2}, 下载进度: {downloadStatus.Percent * 100:F1}%  已下载: {downloadStatus.DownloadedBytes}/{downloadStatus.TotalBytes} 字节"); 
                // 更新进度条显示（考虑起始进度和最大进度限制） 
                //RefreshPrecentProgress(startPercent + curProgress * (loadingMaxPercent - startPercent), downloadStatus.DownloadedBytes, downloadStatus.TotalBytes); 
            }
            else
            {
                // 资源已在本地，直接加载 
                // 资源加载内部可能是分阶段进行的，会出现 Percent 回退，这里限制只允许更大值 
                curProgress = Mathf.Max(curProgress, handle.PercentComplete);
                //DebugLog.LogByTag(TAG, $"场景资源加载进度: {curProgress:P2}"); 
                //RefreshPrecentProgress(startPercent + curProgress * (loadingMaxPercent - startPercent)); 
            }

            yield return null;
        }

        // 检查加载结果 
        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            //DebugLog.LogError($"{sceneFullPath}场景资源加载失败"); 
            // 提示错误并提供重试选项 
            // ErrorTips("加载资源失败", () => 
            // { 
            //     fail?.Invoke(); 
            //     Hide(); 
            // }, () => { loadCoroutine = StartCoroutine(LoadSceneAsync(sceneFullPath, startPercent, finish, success, fail)); }); 
            // Addressables.Release(handle); 
            yield break;
        }

        // 资源加载成功 
        //DebugLog.LogByTag(TAG, $"{sceneFullPath}场景资源加载成功"); 
        //RefreshPrecentProgress(loadingMaxPercent); 
        //RefreshHint("正在加载场景..."); 

        // 等待一段时间执行灭火动画效果 
        yield return new WaitForSeconds(0.7f);
        //RefreshPrecentProgress(1); 
        yield return new WaitForSeconds(0.3f);

        // 执行成功回调 
        success?.Invoke();

        // 激活场景 
        var sceneHandle = handle.Result.ActivateAsync();
        yield return sceneHandle;

        // 设置为活动场景 
        SceneManager.SetActiveScene(handle.Result.Scene);
        //DebugLog.LogByTag(TAG, $"{sceneFullPath}场景激活完成"); 

        // 记录已加载的场景句柄 
        loadedSceneHandles.Add(sceneFullPath, handle);

        yield return null;

        // 执行完成回调 
        finish?.Invoke();
        //Hide(); 
    }

    /// <summary> 
    /// 销毁场景 
    /// </summary> 
    private IEnumerator UnloadSceneAsync(Action callback)
    {
        foreach (var (sceneFullPath, handle) in loadedSceneHandles)
        {
            if (!handle.IsValid()) continue;
            var handel = Addressables.UnloadSceneAsync(handle);
            yield return handel;
            //DebugLog.LogByTag(TAG, $"卸载场景:{sceneFullPath}"); 
        }

        loadedSceneHandles.Clear();

        callback();
    }

    #endregion
    
    #region 加载UI
    
    
    
    #endregion
}