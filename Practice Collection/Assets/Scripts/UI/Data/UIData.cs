
using System;

/// <summary>
/// UI层级定义
/// </summary>
public enum UILayer
{
    Background,     // 背景层（常驻背景）
    Bottom,         // 底层（主界面等）
    Normal,         // 普通层（弹窗、面板）
    Top,            // 顶层（提示、加载）
    Alert,          // 警告层（强制弹窗）
    System          // 系统层（崩溃、更新提示）
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
    None,
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
