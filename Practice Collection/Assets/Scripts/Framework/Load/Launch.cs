using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;

public class Launch : MonoBehaviour
{
    [SerializeField][FieldName("启动界面")] LaunchUI launchUI;
    
    private const float downloadPercent = 0.4f;
    private const float loadSceneAbPercent = 0.38f;
    private const string mainScenePath = "Assets/Scenes/MainScene.unity";
    
    private IEnumerator Start()
    {
        Debug.Log("开始启动流程");
        yield return null;
        StartCoroutine(LoadMainScene());
    }
    
    

    private IEnumerator LoadMainScene()
    {
        //AsyncOperationHandle<SceneInstance>
        var handle = Addressables.LoadSceneAsync(mainScenePath, LoadSceneMode.Single, false); // 异步加载场景
        
        Debug.Log("开始异步加载场景");
        float curProgress = 0;
        while (handle is { IsDone: false })
        {
            curProgress = Mathf.Max(curProgress, handle.PercentComplete);   // 确保进度百分比不会回退
            launchUI.RefreshPrecentProgress(downloadPercent + curProgress * loadSceneAbPercent);
            yield return null;
        }

        if (handle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("场景资源加载失败");
            //弹窗，是否重试？
            AddressableFail(handle, "场景资源加载失败");
            yield break;
        }
        
        launchUI.RefreshPrecentProgress(downloadPercent + loadSceneAbPercent);
        
        var scenehandle = handle.Result.ActivateAsync();
        yield return scenehandle;       //等待场景激活完成
        Addressables.Release(handle);
        Addressables.Release(scenehandle);
        Debug.Log("场景加载完成");
    }
    
    private void AddressableFail(AsyncOperationHandle handle, string error)
    {
        Debug.LogError(error);
        Debug.LogError("AddressableFail Error\n" + handle.OperationException);
        Addressables.Release(handle);
    }
}
