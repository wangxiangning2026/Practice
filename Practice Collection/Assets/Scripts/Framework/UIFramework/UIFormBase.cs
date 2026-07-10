using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
    /// UI界面基类 - 所有UI面板必须继承此类
    /// </summary>
    public abstract class UIFormBase : MonoBehaviour    
    {
        [Header("UI基础配置")]
        [SerializeField] protected UIViewType viewType;         
        [SerializeField] protected UILayer layer = UILayer.Normal;
        
        // 生命周期属性
        public UIViewType ViewType => viewType;
        public UILayer Layer => layer;
        public bool IsInitialized { get; private set; }
        public bool IsVisible => gameObject.activeSelf;
        
        protected virtual void Awake()
        {
         
            // 默认隐藏
            gameObject.SetActive(false);
        }

        #region 生命周期方法（由UIManager调用）

        /// <summary>
        /// 初始化（只执行一次：绑定事件、获取组件）
        /// </summary>
        public virtual void OnInit()
        {
            IsInitialized = true;
            BindUIEvents();
        }

        /// <summary>
        /// 打开界面（每次打开都会调用）
        /// </summary>
        /// <param name="args">传入参数</param>
        public virtual void OnOpen(object args = null)
        {
            gameObject.SetActive(true);
            OnRefresh(args);
        }

        /// <summary>
        /// 刷新界面数据（不重新加载资源，只更新显示）
        /// </summary>
        public virtual void OnRefresh(object args = null)
        {
            // 子类重写实现数据刷新
        }

        /// <summary>
        /// 显示界面
        /// </summary>
        public virtual void ShowView()
        {
            gameObject.SetActive(true);
        }
        
        /// <summary>
        /// 隐藏界面
        /// </summary>
        public virtual void HideView()
        {
            gameObject.SetActive(false);
            UnbindUIEvents();
        }

       

        #endregion

        #region 事件绑定（子类实现）

        /// <summary>
        /// 绑定UI事件（子类重写）
        /// </summary>
        protected virtual void BindUIEvents() { }

        /// <summary>
        /// 解绑UI事件（子类重写）
        /// </summary>
        protected virtual void UnbindUIEvents() { }

        #endregion
        
    }
