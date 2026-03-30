using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class NetWorkManager : MonoBehaviour
{
    // 单例模式
    public static NetWorkManager Instance { get; private set; }

    [Header("服务器配置")] public string serverUrl = "http://localhost:3000"; // 本地服务器
    // public string serverUrl = "http://你的云服务器IP:3000"; // 云服务器

    [Header("玩家信息")] public string playerId;
    public string playerName;
    public int playerScore;
    public int playerLevel;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 场景切换不销毁
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 生成唯一玩家ID（实际项目可以用设备ID或登录账号）
        if (string.IsNullOrEmpty(playerId))
        {
            playerId = SystemInfo.deviceUniqueIdentifier;
            Debug.Log($"玩家ID: {playerId}");
        }
    }

    #region HTTP 请求方法

    /// <summary>
    /// GET 请求
    /// </summary>
    public IEnumerator GetRequest(string endpoint, System.Action<string> onSuccess,
        System.Action<string> onError = null)
    {
        string url = serverUrl + endpoint;
        using (UnityWebRequest request = UnityWebRequest.Get(url))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"GET 成功: {request.downloadHandler.text}");
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError($"GET 失败: {request.error}");
                onError?.Invoke(request.error);
            }
        }
    }

    /// <summary>
    /// POST 请求（JSON 格式）
    /// </summary>
    public IEnumerator PostRequest(string endpoint, object data, System.Action<string> onSuccess,
        System.Action<string> onError = null)
    {
        string url = serverUrl + endpoint;
        string jsonData = JsonUtility.ToJson(data);

        using (UnityWebRequest request = new UnityWebRequest(url, "POST"))
        {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log($"POST 成功: {request.downloadHandler.text}");
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError($"POST 失败: {request.error}");
                onError?.Invoke(request.error);
            }
        }
    }

    /// <summary>
    /// POST 请求（表单格式，简单场景使用）
    /// </summary>
    public IEnumerator PostFormRequest(string endpoint, Dictionary<string, string> formData,
        System.Action<string> onSuccess)
    {
        string url = serverUrl + endpoint;
        WWWForm form = new WWWForm();

        foreach (var kvp in formData)
        {
            form.AddField(kvp.Key, kvp.Value);
        }

        using (UnityWebRequest request = UnityWebRequest.Post(url, form))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                onSuccess?.Invoke(request.downloadHandler.text);
            }
            else
            {
                Debug.LogError($"POST 失败: {request.error}");
            }
        }
    }

    #endregion

    #region 游戏 API 调用

    /// <summary>
    /// 玩家登录/注册
    /// </summary>
    public void Login(System.Action<LoginResponse> onComplete)
    {
        LoginRequest request = new LoginRequest
        {
            playerId = playerId,
            deviceId = SystemInfo.deviceUniqueIdentifier
        };

        StartCoroutine(PostRequest("/api/login", request, (response) =>
        {
            LoginResponse loginResponse = JsonUtility.FromJson<LoginResponse>(response);
            if (loginResponse.success)
            {
                playerId = loginResponse.data.playerId;
                playerName = loginResponse.data.name;
                playerScore = loginResponse.data.score;
                playerLevel = loginResponse.data.level;
                Debug.Log($"登录成功！玩家: {playerName}, 分数: {playerScore}");
            }

            onComplete?.Invoke(loginResponse);
        }));
    }

    /// <summary>
    /// 提交分数
    /// </summary>
    public void SubmitScore(int score, System.Action<ScoreResponse> onComplete)
    {
        ScoreRequest request = new ScoreRequest
        {
            playerId = playerId,
            score = score
        };

        StartCoroutine(PostRequest("/api/submit-score", request, (response) =>
        {
            ScoreResponse scoreResponse = JsonUtility.FromJson<ScoreResponse>(response);
            if (scoreResponse.success)
            {
                playerScore = score;
                Debug.Log($"分数提交成功！排名: {scoreResponse.rank}");
            }

            onComplete?.Invoke(scoreResponse);
        }));
    }

    /// <summary>
    /// 获取排行榜
    /// </summary>
    public void GetLeaderboard(System.Action<LeaderboardResponse> onComplete)
    {
        StartCoroutine(GetRequest("/api/rank", (response) =>
        {
            LeaderboardResponse leaderboard = JsonUtility.FromJson<LeaderboardResponse>(response);
            if (leaderboard.success)
            {
                Debug.Log($"获取排行榜成功，共 {leaderboard.data.Length} 条记录");
            }

            onComplete?.Invoke(leaderboard);
        }));
    }

    /// <summary>
    /// 心跳检测
    /// </summary>
    public void SendHeartbeat()
    {
        HeartbeatRequest request = new HeartbeatRequest
        {
            playerId = playerId,
            timestamp = System.DateTimeOffset.UtcNow.ToUnixTimeSeconds()
        };

        StartCoroutine(PostRequest("/api/heartbeat", request, (response) =>
        {
            // 心跳成功，不需要处理
        }));
    }

    #endregion
}