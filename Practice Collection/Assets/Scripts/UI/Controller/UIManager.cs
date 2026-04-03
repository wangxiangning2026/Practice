using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;


public class UIManager : MonoSingleton<UIManager>
{
    [Header("UI根节点配置")] [SerializeField] private Canvas _rootCanvas;
    [SerializeField] private RectTransform _backgroundRoot;
    [SerializeField] private RectTransform _bottomRoot;
    [SerializeField] private RectTransform _normalRoot;
    [SerializeField] private RectTransform _topRoot;
    [SerializeField] private RectTransform _alertRoot;
    [SerializeField] private RectTransform _systemRoot;

    [Header("性能设置")] [SerializeField] private int _maxCacheCount = 10; // 最大缓存数量
    [SerializeField] private float _cacheCleanInterval = 300f; // 缓存清理间隔（秒）

    // 视图缓存
    private Dictionary<UIViewType, GameObject> _viewPrefabs = new();
    private Dictionary<UIViewType, GameObject> _viewCache = new();
    private Dictionary<UIViewType, AsyncOperationHandle<GameObject>> _loadingHandles = new();

    // 视图栈管理
    private Stack<UIViewBase> _viewStack = new();
    private Dictionary<UILayer, Stack<UIViewBase>> _layerStacks = new();
    private Dictionary<UIViewType, UIViewBase> _activeViews = new();

    // 历史记录（用于返回导航）
    private List<UIViewBase> _navigationHistory = new();
    private const int MaxHistoryCount = 50;     //历史记录最大数量

    // 其他
    private float _lastCleanTime;
    private bool _isTransitioning = false;
    private UIViewBase _currentTopView;

    protected override void Awake()
    {
        base.Awake();
        InitializeLayerStacks();
        StartCoroutine(CacheCleanRoutine());
    }

    //初始化_layerStacks，视图层级栈
    private void InitializeLayerStacks()
    {
        foreach (UILayer layer in Enum.GetValues(typeof(UILayer)))
        {
            _layerStacks[layer] = new Stack<UIViewBase>();
        }
    }

    #region 视图注册与加载

    /// <summary>
    /// 注册视图预制体
    /// </summary>
    public void RegisterView(UIViewType viewName, GameObject prefab)
    {
        if (!_viewPrefabs.TryAdd(viewName, prefab))
        {
            Debug.LogWarning($"视图 {viewName} 已经注册过");
        }
    }

    /// <summary>
    /// 异步打开视图（推荐使用）
    /// </summary>
    public void OpenViewAsync(UIViewType viewName, UILayer layer = UILayer.Normal, UIParams parameters = null, Action<UIViewBase> onComplete = null)
    {
        if (_isTransitioning)
        {
            Debug.LogWarning("正在执行UI过渡动画，请稍后再试");
            return;
        }

        if (_activeViews.ContainsKey(viewName) && _activeViews[viewName].State == UIViewState.Opened)
        {
            Debug.LogWarning($"视图 {viewName} 已经打开");
            onComplete?.Invoke(_activeViews[viewName]);
            return;
        }

        StartCoroutine(OpenViewCoroutine(viewName, layer, parameters, onComplete));
    }

    private IEnumerator OpenViewCoroutine(UIViewType viewName, UILayer layer, UIParams parameters, Action<UIViewBase> onComplete)
    {
        _isTransitioning = true;

        // 处理当前顶层视图的暂停逻辑
        if (_currentTopView != null && _currentTopView.PauseWhenCovered)
        {
            _currentTopView.OnPause();
        }

        // 获取或创建视图
        UIViewBase view = null;
        GameObject viewGO = null;

        // 从缓存获取
        if (_viewCache.TryGetValue(viewName, out viewGO))
        {
            _viewCache.Remove(viewName);
            view = viewGO.GetComponent<UIViewBase>();
            viewGO.SetActive(true);
        }
        else
        {
            // 加载预制体
            if (_viewPrefabs.TryGetValue(viewName, out GameObject prefab))
            {
                viewGO = Instantiate(prefab, GetLayerRoot(layer));
                view = viewGO.GetComponent<UIViewBase>();

                if (view == null)
                {
                    Debug.LogError($"视图 {viewName} 缺少 UIView 组件");
                    _isTransitioning = false;
                    yield break;
                }
            }
            else
            {
                // 通过Addressables加载
                yield return LoadViewFromAddressable(viewName, layer, result =>
                {
                    viewGO = result;
                    view = viewGO?.GetComponent<UIViewBase>();
                });

                if (viewGO == null)
                {
                    Debug.LogError($"无法加载视图 {viewName}");
                    _isTransitioning = false;
                    yield break;
                }
            }
        }

        // 初始化视图
        view.ViewType = viewName;
        view.Layer = layer;
        view.State = UIViewState.Loading;

        // 设置RectTransform
        RectTransform rect = viewGO.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
        rect.localScale = Vector3.one;

        // 执行打开动画前回调
        yield return view.OnBeforeOpen(parameters);

        // 播放打开动画
        yield return view.PlayOpenAnimation();

        // 激活视图
        view.State = UIViewState.Opened;
        _activeViews[viewName] = view;

        // 更新栈
        _layerStacks[layer].Push(view);
        _viewStack.Push(view);
        _currentTopView = view;

        // 记录历史
        if (_navigationHistory.Count >= MaxHistoryCount)
        {
            _navigationHistory.RemoveAt(0);
        }

        _navigationHistory.Add(view);

        // 执行打开后回调
        view.OnAfterOpen(parameters);
        parameters?.OnOpened?.Invoke(view);
        onComplete?.Invoke(view);

        _isTransitioning = false;
    }

    private IEnumerator LoadViewFromAddressable(UIViewType viewName, UILayer layer, Action<GameObject> onComplete)
    {
        var handle = Addressables.LoadAssetAsync<GameObject>(viewName);
        _loadingHandles[viewName] = handle;
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject prefab = handle.Result;
            GameObject viewGO = Instantiate(prefab, GetLayerRoot(layer));
            onComplete?.Invoke(viewGO);
        }
        else
        {
            onComplete?.Invoke(null);
        }

        _loadingHandles.Remove(viewName);
    }

    #endregion

    #region 视图关闭

    /// <summary>
    /// 关闭指定视图
    /// </summary>
    public void CloseView(UIViewType viewName, bool animated = true)
    {
        if (!_activeViews.TryGetValue(viewName, out UIViewBase view))
        {
            Debug.LogWarning($"视图 {viewName} 未打开");
            return;
        }

        StartCoroutine(CloseViewCoroutine(view, animated));
    }

    /// <summary>
    /// 关闭当前顶层视图
    /// </summary>
    public void CloseCurrentView(bool animated = true)
    {
        if (_currentTopView != null)
        {
            CloseView(_currentTopView.ViewType, animated);
        }
    }

    /// <summary>
    /// 关闭指定层级的所有视图
    /// </summary>
    public void CloseAllViewsInLayer(UILayer layer, bool animated = true)
    {
        var stack = _layerStacks[layer];
        var viewsToClose = new List<UIViewBase>(stack);

        foreach (var view in viewsToClose)
        {
            CloseView(view.ViewType, animated);
        }
    }

    /// <summary>
    /// 关闭所有视图（除了常驻视图）
    /// </summary>
    public void CloseAllViews(bool animated = true)
    {
        var viewsToClose = new List<UIViewBase>(_activeViews.Values);

        foreach (var view in viewsToClose)
        {
            if (!view.IsPersistent)
            {
                CloseView(view.ViewType, animated);
            }
        }
    }

    private IEnumerator CloseViewCoroutine(UIViewBase view, bool animated)
    {
        if (view.State != UIViewState.Opened)
            yield break;

        view.State = UIViewState.Closing;

        // 执行关闭前回调
        yield return view.OnBeforeClose();

        // 播放关闭动画
        if (animated)
        {
            yield return view.PlayCloseAnimation();
        }

        // 从栈中移除
        var layerStack = _layerStacks[view.Layer];
        if (layerStack.Count > 0 && layerStack.Peek() == view)
        {
            layerStack.Pop();
        }

        _viewStack.Clear(); // 重建栈（简单处理，实际需要更复杂的逻辑）
        _activeViews.Remove(view.ViewType);

        // 缓存或销毁
        if (view.Cacheable && _viewCache.Count < _maxCacheCount)
        {
            view.gameObject.SetActive(false);
            _viewCache[view.ViewType] = view.gameObject;
            view.State = UIViewState.Closed;
        }
        else
        {
            Destroy(view.gameObject);
        }

        // 恢复被覆盖的视图
        if (layerStack.Count > 0)
        {
            var previousView = layerStack.Peek();
            if (previousView != null && previousView.PauseWhenCovered)
            {
                previousView.OnResume();
            }

            _currentTopView = previousView;
        }
        else
        {
            // 从其他层级找顶层视图
            _currentTopView = FindTopMostView();
        }

        // 执行关闭后回调
        view.OnAfterClose();
        view.Parameters?.OnClosed?.Invoke(view);
    }

    private UIViewBase FindTopMostView()
    {
        for (int i = (int)UILayer.System; i >= (int)UILayer.Background; i--)
        {
            var stack = _layerStacks[(UILayer)i];
            if (stack.Count > 0)
                return stack.Peek();
        }

        return null;
    }

    #endregion

    #region 导航与历史

    /// <summary>
    /// 返回上一页
    /// </summary>
    public void GoBack()
    {
        if (_navigationHistory.Count >= 2)
        {
            var current = _navigationHistory[_navigationHistory.Count - 1];
            var previous = _navigationHistory[_navigationHistory.Count - 2];

            CloseView(current.ViewType);

            // 可选：重新激活上一个视图（如果被销毁了）
            if (previous.State != UIViewState.Opened)
            {
                OpenViewAsync(previous.ViewType, previous.Layer);
            }
        }
    }

    /// <summary>
    /// 清除导航历史
    /// </summary>
    public void ClearHistory()
    {
        _navigationHistory.Clear();
    }

    #endregion

    #region 查询接口

    /// <summary>
    /// 获取视图（如果打开）
    /// </summary>
    public T GetView<T>(UIViewType viewName) where T : UIViewBase
    {
        if (_activeViews.TryGetValue(viewName, out UIViewBase view))
        {
            return view as T;
        }

        return null;
    }

    /// <summary>
    /// 判断视图是否打开
    /// </summary>
    public bool IsViewOpen(UIViewType viewName)
    {
        return _activeViews.ContainsKey(viewName) && _activeViews[viewName].State == UIViewState.Opened;
    }

    /// <summary>
    /// 获取当前顶层视图
    /// </summary>
    public UIViewBase GetCurrentView()
    {
        return _currentTopView;
    }

    #endregion

    #region 缓存管理

    private IEnumerator CacheCleanRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_cacheCleanInterval);
            CleanCache();
        }
    }

    private void CleanCache()
    {
        if (_viewCache.Count <= _maxCacheCount)
            return;

        int toRemove = _viewCache.Count - _maxCacheCount;
        var keysToRemove = new List<UIViewType>();

        foreach (var kvp in _viewCache)
        {
            keysToRemove.Add(kvp.Key);
            if (keysToRemove.Count >= toRemove)
                break;
        }

        foreach (var key in keysToRemove)
        {
            if (_viewCache.TryGetValue(key, out GameObject go))
            {
                Destroy(go);
                _viewCache.Remove(key);
            }
        }

        Debug.Log($"[UIManager] 清理了 {toRemove} 个缓存视图");
    }

    /// <summary>
    /// 手动清理缓存
    /// </summary>
    public void ClearCache()
    {
        foreach (var kvp in _viewCache)
        {
            if (kvp.Value != null)
                Destroy(kvp.Value);
        }

        _viewCache.Clear();
    }

    #endregion

    #region 工具方法

    private Transform GetLayerRoot(UILayer layer)
    {
        switch (layer)
        {
            case UILayer.Background: return _backgroundRoot;
            case UILayer.Bottom: return _bottomRoot;
            case UILayer.Normal: return _normalRoot;
            case UILayer.Top: return _topRoot;
            case UILayer.Alert: return _alertRoot;
            case UILayer.System: return _systemRoot;
            default: return _normalRoot;
        }
    }

    /// <summary>
    /// 获取根Canvas
    /// </summary>
    public Canvas RootCanvas => _rootCanvas;

    #endregion

    #region 生命周期

    private void Update()
    {
        // 可以在这里处理全局输入，如返回键
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GoBack();
        }
    }

    protected override void OnDestroy()
    {
        // 释放Addressables句柄
        foreach (var handle in _loadingHandles.Values)
        {
            if (handle.IsValid())
                Addressables.Release(handle);
        }

        ClearCache();
        base.OnDestroy();
    }

    #endregion

}