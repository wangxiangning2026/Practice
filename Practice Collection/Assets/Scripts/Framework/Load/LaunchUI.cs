
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LaunchUI : MonoBehaviour
{
    [Header("===== 进度条设置 =====")]
    [SerializeField][FieldName("进度条")] private Slider progressSlider;
    [FieldName("进度百分比")][SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private TextMeshProUGUI titleText;
    [FieldName("提示")][SerializeField] private TextMeshProUGUI tipText;
    [SerializeField] private TextMeshProUGUI subText;
    
    private  float totalProgress;


    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        progressSlider.value = 0f;
        progressText.text = "0%";
    }
    
    private const float BytesToMB = 1048576f;
    private const float BytesToKB = 1024f;
    private float downloadSpeed;
    private float downloadedBytes;
    private float lastDownloadedTime;
    
    /// <summary>
    /// 刷新热更新进度
    /// </summary>
    /// <param name="curBytes">已下载多少 MB</param>
    /// <param name="totalBytes">需要下载多少 MB</param>
    /// <param name="percent">总进度（范围：0.0 ~ 1.0）</param>
    public void RefreshPrecentProgress(float percent, float curBytes = 0f, float totalBytes = 0f)
    {
        RefreshProgress(percent);

        if (!(totalBytes > 0)) return;
        string hint = $"系统内容更新，请耐心等待！\n努力下载更新中:{curBytes / BytesToMB:F2}MB / {totalBytes / BytesToMB:F2}MB";

        // 统计一秒内的下载速度
        var time = Time.time - lastDownloadedTime;
        // 如果累计下载量为0，需要计算下载速度，否则每一秒更新下载速度
        if (downloadedBytes == 0 || time > 1f)
        {
            // 计算一秒内下载量
            // 资源下载失败可能会发起重试，导致传入的已下载字节数比上一次更小，所以这里取最大值
            var curLoaded = Mathf.Max(0f, curBytes - downloadedBytes);
            downloadSpeed = curLoaded / time;

            // 如果时间间隔大于1秒，则更新累计下载量
            if (time > 1f)
            {
                lastDownloadedTime = Time.time;
                downloadedBytes = curBytes;
            }
        }

        var speed = downloadSpeed / BytesToKB;
        string speedUnit = "KB/秒";
        if (speed >= BytesToKB)
        {
            speed /= BytesToKB;
            speedUnit = "MB/秒";
        }

        var speedHint = $"  {speed:F2} {speedUnit}";
        Debug.Log(speedHint);
        hint += speedHint;

        RefreshHint(hint);
    }
    
    /// <summary>
    /// 刷新进度
    /// </summary>
    /// <param name="progress">当前进度，取值范围：[0, 1]</param>
    public void RefreshProgress(float progress)
    {
        totalProgress = progress;
        progressSlider.value = totalProgress;
        progressText.text = $"{(int)(progressSlider.value * 100)}%";
    }
    
    /// <summary>
    /// 刷新提示语
    /// </summary>
    /// <param name="hint"></param>
    public void RefreshHint(string hint)
    {
        tipText.text = hint;
    }
}
