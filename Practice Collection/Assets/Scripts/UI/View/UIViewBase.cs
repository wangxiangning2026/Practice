using System.Collections;
using UnityEngine;

public class UIViewBase : MonoBehaviour
{
    [Header("视图配置")]
    [SerializeField] private bool _isPersistent = false;      // 是否常驻（不随CloseAll关闭）
    [SerializeField] private bool _cacheable = true;           // 是否可缓存
    [SerializeField] private bool _pauseWhenCovered = true;    // 被覆盖时是否暂停
    [SerializeField] private float _openAnimDuration = 0.3f;
    [SerializeField] private float _closeAnimDuration = 0.3f;
    
    public UIViewType ViewType { get; set; }
    
    public UILayer Layer { get; set; }
    public UIViewState State { get; set; } = UIViewState.None;
    public UIParams Parameters { get; private set; }
        
    public bool IsPersistent => _isPersistent;
    public bool Cacheable => _cacheable;
    public bool PauseWhenCovered => _pauseWhenCovered;
        
    // 子类可重写的生命周期回调
    public virtual IEnumerator OnBeforeOpen(UIParams parameters)
    {
        Parameters = parameters;
        yield break;
    }
        
    public virtual void OnAfterOpen(UIParams parameters) { }
        
    public virtual IEnumerator OnBeforeClose()
    {
        yield break;
    }
        
    public virtual void OnAfterClose() { }
        
    public virtual void OnPause() { }
        
    public virtual void OnResume() { }
        
    // 默认动画（使用DOTween）
    public virtual IEnumerator PlayOpenAnimation()
    {
        transform.localScale = Vector3.zero;
        //Tweener tweener = transform.DOScale(1f, _openAnimDuration).SetEase(Ease.OutBack);
        //yield return tweener.WaitForCompletion();
        yield return null;
    }
        
    public virtual IEnumerator PlayCloseAnimation()
    {
        //Tweener tweener = transform.DOScale(0f, _closeAnimDuration).SetEase(Ease.InBack);
        //yield return tweener.WaitForCompletion();
        yield return null;
    }

}
