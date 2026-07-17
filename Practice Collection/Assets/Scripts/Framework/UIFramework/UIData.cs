
using System;

/// <summary>
/// UI层级定义
/// </summary>
public enum UILayer
{
    Background = 0,   // 背景层（排序值 0-19）
    Normal = 100,     // 普通界面层（100-119）
    Popup = 200,      // 弹窗层（200-219）
    Topmost = 300,    // 顶层（300-319）
    Loading = 400,    // 加载遮罩层（400-419）
    Toast = 500       // 提示层（500-519，永远在最上面）
}

/// <summary>
/// 存储每个UI界面的运行时信息
/// </summary>
public class UIViewInfo
{
    public UIViewType ViewType;           // 界面名称（唯一标识）
    public UIViewBase ViewInstance;       // 界面实例
    public UILayer Layer;             // 所属层级
}

/// <summary>
/// UI视图状态
/// </summary>
public enum UIViewState
{
    None,
    Loading,
    Opened,
    Closing,
    Closed
}

public enum UIViewType
{
    None = 0,
    LoadView = 1,  //登录界面
    MainView = 2,   //主界面
}

/// <summary>
/// UI打开参数基类
/// </summary>
public class UIParams
{
    public object UserData { get; set; }
    public Action<UIViewBase> OnOpened { get; set; }
    public Action<UIViewBase> OnClosed { get; set; }
}
