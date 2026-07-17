using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI管理器 - 负责所有UI的加载、显示、关闭、层级管理
/// </summary>
public class UIManager : MonoSingleton<UIManager>
{
    [SerializeField]
    [FieldName("View预制体")] public Dictionary<UIViewType, GameObject> viewPrefab = new ();
    private UIViewInfo currentView;
    
    // 所有已加载的UI
    private Dictionary<UIViewType, UIViewInfo> loadedViews = new Dictionary<UIViewType, UIViewInfo>();

    private Dictionary<UIViewType, GameObject> InitViews = new();

    // UI栈（用于管理界面返回逻辑）
    private Stack<UIViewInfo> viewStack = new Stack<UIViewInfo>();

    #region 打开UI（核心方法）

    /// <summary>
    /// UI Adressable 异步加载
    /// </summary>
    public void OpenUIAsync<T>(object args = null, Action<T> onComplete = null, bool usePool = false) where T : UIViewBase
    {
        UIViewType viewType = (UIViewType)Enum.Parse(typeof(UIViewType),typeof(T).Name);
        Type type = typeof(T);
        string formName = type.Name;
        string addressableKey = GetAddressableKey(type);

        if(loadedViews.TryGetValue(viewType, out UIViewInfo viewInfo))
        {
            if (currentView.ViewType == viewType)
            {
                viewInfo.ViewInstance.OnRefresh();
                return;
            }
            ChangeView(viewInfo);
            return;
        }
        
        // 异步加载
        ResourceManager.Instance.LoadAssetAsync(addressableKey, (instance) =>
        {
            if (instance == null)
            {
                onComplete?.Invoke(null);
                return;
            }

            T form = instance.GetComponent<T>();
            if (form == null)
            {
                form = instance.AddComponent<T>();
            }

            // 后续初始化流程与同步加载相同
            InitializeView(form);
            onComplete?.Invoke(form);
        });
    }

    /// <summary>
    /// UI prefab 实例化
    /// </summary>
    public void OpenUI<T>()
    {
        UIViewType viewType = (UIViewType)Enum.Parse(typeof(UIViewType), typeof(T).Name);
        if(loadedViews.TryGetValue(viewType, out UIViewInfo viewInfo))
        {
            if (currentView.ViewType == viewType)
            {
                viewInfo.ViewInstance.OnRefresh();
                return;
            }
            ChangeView(viewInfo);
            return;
        }
        if(viewPrefab.TryGetValue(viewType,out var prefab))
        {
            UIViewBase view = Instantiate(prefab, transform).GetComponent<UIViewBase>();
            InitializeView(view);
        }
    }
    
    private string GetAddressableKey(Type type)
    {
        // Addressables的Key可以是资源路径或Label
        // 这里使用类名作为Key，需要在Addressables中标记
        return type.Name;
    }
    
    private void InitializeView<T>(T view) where T : UIViewBase
    {
        UILayer layer = view.Layer;
        UIViewInfo info = new UIViewInfo
        {
            ViewType = (UIViewType)Enum.Parse(typeof(UIViewType),typeof(T).Name),
            ViewInstance = view,
            Layer = layer,
        };
        loadedViews[info.ViewType] = info;
        
        ChangeView(info);
        
        Debug.Log($"[UIManager] 异步打开UI: {info.ViewType}");
    }

    private void ChangeView(UIViewInfo targetView)
    {
        // 1. 暂停当前面板
        if (currentView != null && currentView.ViewType != targetView.ViewType)
        {
            // 判断是否压入栈（弹窗类需要压栈）
            if (ShouldPushToStack(targetView))
            {
                viewStack.Push(targetView);
                currentView = targetView;
                currentView.ViewInstance.HideView();
                targetView.ViewInstance.ShowView();
                Debug.Log($"[UIManager] 切换到面板: {targetView.ViewType}");
            }
        }
    }

    #endregion
    
    #region 辅助方法

    /// <summary>
    /// 判断是否需要压入栈
    /// </summary>
    private bool ShouldPushToStack(UIViewInfo form)
    {
        // 背景层和Toast层不入栈
        return form.Layer != UILayer.Background && form.Layer != UILayer.Toast;
    }

    #endregion
    
}